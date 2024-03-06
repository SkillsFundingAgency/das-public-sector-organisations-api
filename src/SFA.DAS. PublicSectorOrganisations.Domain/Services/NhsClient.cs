//using Newtonsoft.Json;
//using System.Web;
//using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

//namespace SFA.DAS.PublicSectorOrganisations.Domain.Services;


//public class NhsClient : INhsClient
//{
//    private readonly HttpClient _client;

//    public NhsClient(HttpClient client)
//    {
//        _client = client;
//    }
//    public async Task<GetAllOrganisationsResponse> GetAllOrganisations(string sector)
//    {
//        var response = await _client.GetStringAsync("ORD/2-0-0/organisations?Limit=1000&Status=Active&PrimaryRoleId=" + HttpUtility.UrlEncode(sector));
//        var result = JsonConvert.DeserializeObject<GetAllOrganisationsResponse>(response);
//        return result;
//    }

//    public async Task<GetOrganisationResponse> GetOrganisation(string orgId)
//    {
//        var response = await _client.GetStringAsync("ORD/2-0-0/organisations/" + HttpUtility.UrlEncode(orgId));
//        var result = JsonConvert.DeserializeObject<GetOrganisationResponse>(response);
//        return result;
//    }
//}

//public class GetAllOrganisationsResponse
//{
//    public OrganisationSummary[] Organisations { get; set; }
//}

//public class GetOrganisationResponse
//{
//    public OrganisationDetails Organisation { get; set; }
//}

//public class OrganisationSummary
//{
//    public string Name { get; set; }
//    public string OrgId { get; set; }
//    public string OrgRecordClass { get; set; }
//    public string PostCode { get; set; }
//    public string PrimaryRoleId { get; set; }
//    public string PrimaryRoleDescription { get; set; }
//    public string OrgLink { get; set; }
//    public string UPRN { get; set; }

//}

//public class OrganisationDetails
//{

//    public string Name { get; set; }
//    public GeoLoc GeoLoc { get; set; }
//}

//public class GeoLoc
//{
//    public Location Location { get; set; }
//}

//public class Location
//{
//    public string AddrLn1 { get; set; }
//    public string AddrLn2 { get; set; }
//    public string AddrLn3 { get; set; }
//    public string Town { get; set; }
//    public string PostCode { get; set; }
//    public string Country { get; set; }
//    public string UPRN { get; set; }
//}



