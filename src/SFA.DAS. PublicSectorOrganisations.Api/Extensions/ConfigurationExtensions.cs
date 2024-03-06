namespace SFA.DAS.PublicSectorOrganisations.Api.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsLocalOrDev(this IConfiguration configuration)
        => configuration["EnvironmentName"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
           configuration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);

    public static bool IsDev(this IConfiguration configuration)
        => configuration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);

}

