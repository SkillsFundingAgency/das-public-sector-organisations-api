using System.Web;
using Newtonsoft.Json;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Data.NhsSearch;


public class NhsClient : INhsClient
{
    private readonly HttpClient _client;

    public NhsClient(HttpClient client)
    {
        _client = client;
    }
    public async Task<GetAllOrganisationsResponse> GetAllOrganisations(string sector)
    {
        var response = await _client.GetStringAsync("ORD/2-0-0/organisations?Limit=1000&Status=Active&PrimaryRoleId=" + HttpUtility.UrlEncode(sector));
        var result = JsonConvert.DeserializeObject<GetAllOrganisationsResponse>(response);
        return result;
    }

    public async Task<GetSingleOrganisationResponse> GetOrganisation(string orgId)
    {
        var response = await _client.GetStringAsync("ORD/2-0-0/organisations/" + HttpUtility.UrlEncode(orgId));
        var result = JsonConvert.DeserializeObject<GetOrganisationResponse>(response);

        return new GetSingleOrganisationResponse
        {
            AddressLine1 = result.Organisation?.GeoLoc?.Location?.AddrLn1,
            AddressLine2 = result.Organisation?.GeoLoc?.Location?.AddrLn2,
            AddressLine3 = result.Organisation?.GeoLoc?.Location?.AddrLn3,
            Town = result.Organisation?.GeoLoc?.Location?.Town,
            PostCode = result.Organisation?.GeoLoc?.Location?.PostCode,
            Country = result.Organisation?.GeoLoc?.Location?.Country,
            UPRN = result.Organisation?.GeoLoc?.Location?.UPRN,
        };
    }
}

class GetOrganisationResponse
{
    public OrganisationDetails Organisation { get; set; }
}

class GeoLoc
{
    public Location Location { get; set; }
}

class Location
{
    public string AddrLn1 { get; set; }
    public string AddrLn2 { get; set; }
    public string AddrLn3 { get; set; }
    public string Town { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
    public string UPRN { get; set; }
}

class OrganisationDetails
{

    public string Name { get; set; }
    public GeoLoc GeoLoc { get; set; }
}