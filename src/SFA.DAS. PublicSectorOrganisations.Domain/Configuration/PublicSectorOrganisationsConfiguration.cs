namespace SFA.DAS.PublicSectorOrganisations.Domain.Configuration;

public class PublicSectorOrganisationsConfiguration
{
    public string ConnectionString { get; set; }

    public string PoliceForceUrl { get; set; }
    public string NhsUrl { get; set; }
    public NhsSector[] NhsSectors { get; set; } = Array.Empty<NhsSector>();
}

public class NhsSector
{
    public string ExternalCode { get; set; }
    public string InternalCode { get; set; }
}