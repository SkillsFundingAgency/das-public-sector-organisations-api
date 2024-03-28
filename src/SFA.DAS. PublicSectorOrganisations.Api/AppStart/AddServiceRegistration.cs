using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Data;
using SFA.DAS.PublicSectorOrganisations.Data.Nhs;
using SFA.DAS.PublicSectorOrganisations.Data.Ons;
using SFA.DAS.PublicSectorOrganisations.Data.Police;
using SFA.DAS.PublicSectorOrganisations.Data.Providers;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, PublicSectorOrganisationsConfiguration config)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ImportCommand).Assembly));
        services.AddTransient<IImporterService, PoliceImporterService>();
        services.AddTransient<IImporterService, NhsImporterService>();
        services.AddTransient<IImporterService, OnsImporterService>();
        services.AddHttpClient<IPoliceApiClient, PoliceApiClient>(client => client.BaseAddress = new Uri(config.PoliceForceUrl));
        services.AddTransient<IOnsDownloadService, OnsDownloadService>();
        services.AddHttpClient<IOnsDownloadClient, OnsDownloadClient>();
        services.AddTransient<IOnsExcelReaderService, OnsExcelReaderService>();
        services.AddHttpClient<INhsApiClient, NhsApiClient>(client => client.BaseAddress = new Uri(config.NhsUrl));
        services.AddTransient<IPublicSectorOrganisationRepository, PublicSectorOrganisationRepository>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
    }
}