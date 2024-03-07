namespace SFA.DAS.PublicSectorOrganisations.Domain.Entities;

public class GetSingleOrganisationResponse
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string Town { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
    public string UPRN { get; set; }
}