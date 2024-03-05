using RestEase.HttpClientFactory;
using SFA.DAS.PublicSectorOrganisations.Domain.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Domain.Services;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        // services.AddScoped<IPublicSectorOrganisationRepository, PublicSectorOrganisationRepository>();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ImportCommand).Assembly));
        services.AddHttpClient<INhsClient, NhsClient>(client => client.BaseAddress = new Uri("https://directory.spineservices.nhs.uk"));
        services.AddTransient<INhsImporterService, NhsImporterService>();
    }
}