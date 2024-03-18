using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data;

public class PublicSectorOrganisationRepository : IPublicSectorOrganisationRepository
{
    private readonly Lazy<PublicSectorOrganisationDataContext> _dbContext;
    private readonly ILogger<PublicSectorOrganisationRepository> _logger;

    public PublicSectorOrganisationRepository(Lazy<PublicSectorOrganisationDataContext> dbContext,
        ILogger<PublicSectorOrganisationRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsFor(DataSource dataSource)
    {
        _logger.LogInformation("Getting organisations for {source}", dataSource);
        var db = _dbContext.Value;
        return db.PublicSectorOrganisationEntities.Where(x => x.Source == dataSource).ToListAsync();
    }

    public async Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource,
        IEnumerable<PublicSectorOrganisationEntity> updates, IEnumerable<PublicSectorOrganisationEntity> adds)
    {
        var db = _dbContext.Value;
        await db.ExecuteInATransaction(async () =>
        {
            var toUpdate = updates.ToList();
            var toAdd = adds.ToList();
            _logger.LogInformation("Updating {existingCount} and adding {newCount} for data source '{source}'",
                toUpdate.Count, toAdd.Count, dataSource);
            await db.PublicSectorOrganisationEntities.Where(x => x.Source == dataSource)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
            db.PublicSectorOrganisationEntities.UpdateRange(toUpdate);
            await db.PublicSectorOrganisationEntities.AddRangeAsync(toAdd);
        });
    }
}
