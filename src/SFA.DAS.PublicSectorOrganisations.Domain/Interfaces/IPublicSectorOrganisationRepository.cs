using System.Collections.Concurrent;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
public interface IPublicSectorOrganisationRepository
{
    Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsFor(DataSource dataSource);
    Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, ConcurrentBag<PublicSectorOrganisationEntity> toUpdate, ConcurrentBag<PublicSectorOrganisationEntity> toAdd);
}

