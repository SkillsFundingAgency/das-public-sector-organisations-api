using SFA.DAS.PublicSectorOrganisations.Domain.NhsClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IOnsDownloadClient
{
    Task<HttpResponseMessage> GetAsync(string? requestUri);
}