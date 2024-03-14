using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Data;
using SFA.DAS.PublicSectorOrganisations.Data.Nhs;
using SFA.DAS.PublicSectorOrganisations.Data.Police;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, PublicSectorOrganisationsConfiguration config)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ImportCommand).Assembly));
        services.AddHttpClient<IPoliceApiClient, PoliceApiClient>(client => client.BaseAddress = new Uri(config.PoliceForceUrl));
        services.AddTransient<IPoliceImporterService, PoliceImporterService>();
        services.AddTransient<INhsImporterService, NhsImporterService>();
        services.AddTransient<IPublicSectorOrganisationRepository, PublicSectorOrganisationRepository>();
        services.AddHttpClient<INhsApiClient, NhsApiClient>(client => client.BaseAddress = new Uri(config.NhsUrl));
    }
}