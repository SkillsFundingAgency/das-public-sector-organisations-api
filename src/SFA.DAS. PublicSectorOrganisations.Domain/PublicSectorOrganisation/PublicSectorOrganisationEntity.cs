using SFA.DAS.PublicSectorOrganisations.Domain.Models;

namespace SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

public class PublicSectorOrganisationEntity
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public DataSource Source { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? Town { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? UPRN { get; set; }
    public string OrganisationCode { get; set; }
    public bool Active { get; set; }

}