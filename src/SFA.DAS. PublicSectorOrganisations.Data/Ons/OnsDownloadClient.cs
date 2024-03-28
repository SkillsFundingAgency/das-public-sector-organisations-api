using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Data.Ons;

public class OnsDownloadClient : IOnsDownloadClient
{
    private readonly HttpClient _client;

    public OnsDownloadClient(HttpClient client)
    {
        _client = client;
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
    }

    public Task<HttpResponseMessage> GetAsync(string? requestUri)
    {
        return _client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
    }
}

