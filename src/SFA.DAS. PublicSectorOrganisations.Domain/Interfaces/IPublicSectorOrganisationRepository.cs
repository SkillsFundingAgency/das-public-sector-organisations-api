using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
public interface IPublicSectorOrganisationRepository
{
    Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsFor(DataSource dataSource);
    Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, IEnumerable<PublicSectorOrganisationEntity> toUpdate, IEnumerable<PublicSectorOrganisationEntity> toAdd);
}