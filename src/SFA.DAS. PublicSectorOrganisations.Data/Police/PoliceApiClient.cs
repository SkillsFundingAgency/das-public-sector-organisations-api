using Newtonsoft.Json;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Data.Nhs;

public class PoliceApiClient : IPoliceApiClient
{
    private readonly HttpClient _client;

    public PoliceApiClient(HttpClient client)
    {
        _client = client;
    }
    public async Task<PoliceForce[]> GetAllPoliceForces()
    {
        var response = await _client.GetStringAsync("");
        var result = JsonConvert.DeserializeObject<PoliceForce[]>(response);
        return result;
    }

}

