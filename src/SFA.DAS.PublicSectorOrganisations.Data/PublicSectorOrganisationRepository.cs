using System.Runtime.InteropServices.JavaScript;
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

    public Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsForDataSource(DataSource dataSource)
    {
        _logger.LogInformation("Getting organisations for {source}", dataSource);
        var db = _dbContext.Value;
        return db.PublicSectorOrganisationEntities.Where(x => x.Source == dataSource).OrderBy(x=>x.Name).ToListAsync();
    }

    public Task<List<PublicSectorOrganisationEntity>> GetAllActivePublicSectorOrganisations()
    {
        _logger.LogInformation("Getting all organisations");
        var db = _dbContext.Value;
        return db.PublicSectorOrganisationEntities.Where(x => x.Active == true).OrderBy(x => x.Name).ToListAsync();
    }

    public Task<List<PublicSectorOrganisationEntity>> GetMatchingActivePublicSectorOrganisations(string search)
    {
        _logger.LogInformation("Getting matches organisations");
        var db = _dbContext.Value;
        return db.PublicSectorOrganisationEntities.Where(x => x.Active == true && EF.Functions.Like(x.Name, $"%{search}%"))
            .OrderBy(x => x.Name).ToListAsync();
    }

    public Task<PublicSectorOrganisationEntity?> GetPublicSectorOrganisationById(Guid id)
    {
        _logger.LogInformation("Getting organisations by id {id}", id);
        var db = _dbContext.Value;
        return db.PublicSectorOrganisationEntities.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, IEnumerable<PublicSectorOrganisationEntity> updates, IEnumerable<PublicSectorOrganisationEntity> adds, DateTime startTime)
    {
        var db = _dbContext.Value;

        await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
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
                await AddAuditRecord(dataSource, toUpdate.Count, toAdd.Count, startTime);
            });
        });
    }

    private async Task AddAuditRecord(DataSource dataSource, long updateCount, long addCount, DateTime startTime)
    {
        var finishTime = DateTime.UtcNow;
        await _dbContext.Value.ImportAuditEntities.AddAsync(new ImportAuditEntity
        {
            TimeStarted = startTime,
            TimeFinished = finishTime,
            RowsAdded = addCount,
            RowsUpdated = updateCount,
            Source = dataSource
        });
    }
}
