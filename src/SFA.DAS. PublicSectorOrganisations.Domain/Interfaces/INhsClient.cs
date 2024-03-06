using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Services;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface INhsClient
{
    Task<GetAllOrganisationsResponse> GetAllOrganisations(string sector);
    Task<GetSingleOrganisationResponse> GetOrganisation(string orgId);
}