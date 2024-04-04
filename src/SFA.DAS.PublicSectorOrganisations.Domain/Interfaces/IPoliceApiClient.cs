using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IPoliceApiClient
{
    Task<PoliceForce[]> GetAllPoliceForces();
}