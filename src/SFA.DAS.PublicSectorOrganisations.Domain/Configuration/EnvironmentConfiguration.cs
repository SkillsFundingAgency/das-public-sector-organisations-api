namespace SFA.DAS.PublicSectorOrganisations.Domain.Configuration;

public class EnvironmentConfiguration(string environmentName)
{
    public string EnvironmentName { get;} = environmentName;
}