using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddTelemetryRegistrationExtension
{
    public static IServiceCollection AddTelemetryRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var appInsightsConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            services.AddOpenTelemetry().UseAzureMonitor(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            });
        }

        return services;
    }
}
