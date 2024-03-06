using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.PublicSectorOrganisations.Api.AppStart;
using SFA.DAS.PublicSectorOrganisations.Api.Extensions;
using SFA.DAS.PublicSectorOrganisations.Api.Infrastructure;
using SFA.DAS.PublicSectorOrganisations.Data;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

var rootConfiguration = builder.Configuration.LoadConfiguration();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddOptions();
builder.Services.Configure<PublicSectorOrganisationsConfiguration>(rootConfiguration.GetSection(nameof(PublicSectorOrganisationsConfiguration)));
builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<PublicSectorOrganisationsConfiguration>>()!.Value);

builder.Services.AddServiceRegistration();

var publicSectorOrganisationsConfiguration = rootConfiguration
    .GetSection(nameof(PublicSectorOrganisationsConfiguration))
    .Get<PublicSectorOrganisationsConfiguration>();
builder.Services.AddDatabaseRegistration(publicSectorOrganisationsConfiguration!, rootConfiguration["EnvironmentName"]);

if (!rootConfiguration.IsDev())
{
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<PublicSectorOrganisationDataContext>();
}

if (!rootConfiguration.IsLocalOrDev())
{
    var azureAdConfiguration = rootConfiguration
        .GetSection("AzureAd")
        .Get<AzureActiveDirectoryConfiguration>();

    var policies = new Dictionary<string, string>
    {
        {PolicyNames.Default, RoleNames.Default},
    };
    builder.Services.AddAuthentication(azureAdConfiguration, policies);
}

builder.Services.AddControllers(o =>
{
    if (!rootConfiguration.IsLocalOrDev())
    {
        o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
    }
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).AddNewtonsoftJson(o =>
{
    o.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublicSectorOrganisationsApi", Version = "v1" });
    c.OperationFilter<SwaggerVersionHeaderFilter>();
    c.DocumentFilter<JsonPatchDocumentFilter>();
});

builder.Services.AddApiVersioning(opt =>
{
    opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
});

builder.Services.AddLogging();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();

if (!app.Configuration.IsDev())
{
    app.UseHealthChecks();
}

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapDefaultControllerRoute();
});

app.Run();
