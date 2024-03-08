using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Data.Nhs;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, PublicSectorOrganisationsConfiguration config)
    {
        // services.AddScoped<IPublicSectorOrganisationRepository, PublicSectorOrganisationRepository>();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ImportCommand).Assembly));
        services.AddHttpClient<INhsClient, NhsClient>(client => client.BaseAddress = new Uri(config.NhsUrl));
        services.AddTransient<INhsImporterService, NhsImporterService>();
    }
}