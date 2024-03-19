using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
public interface IPublicSectorOrganisationRepository
{
    Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsForDataSource(DataSource dataSource);
    Task<List<PublicSectorOrganisationEntity>> GetAllActivePublicSectorOrganisations();
    Task<PublicSectorOrganisationEntity?> GetPublicSectorOrganisationById(Guid id);
    Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, IEnumerable<PublicSectorOrganisationEntity> toUpdate, IEnumerable<PublicSectorOrganisationEntity> toAdd);
}