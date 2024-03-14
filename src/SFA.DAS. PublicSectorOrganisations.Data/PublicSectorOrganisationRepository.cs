using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data;

public class PublicSectorOrganisationRepository : IPublicSectorOrganisationRepository
{
    private readonly Lazy<PublicSectorOrganisationDataContext> _dbContext;
    private readonly ILogger<PublicSectorOrganisationRepository> _logger;

    public PublicSectorOrganisationRepository(Lazy<PublicSectorOrganisationDataContext> dbContext, ILogger<PublicSectorOrganisationRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsFor(DataSource dataSource)
    {
        return _dbContext.Value.PublicSectorOrganisationEntities.Where(x => x.Source == dataSource).AsNoTracking().ToListAsync();
    }

    public async Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, ConcurrentBag<PublicSectorOrganisationEntity> toUpdate, ConcurrentBag<PublicSectorOrganisationEntity> toAdd)
    {
        var db = _dbContext.Value;
        await db.ExecuteInATransaction(async () =>
        {
            _logger.LogInformation("Updating {existingCount} and adding {newCount} NHS Organisations", toUpdate.Count, toAdd.Count);
            await db.PublicSectorOrganisationEntities.Where(x => x.Source == DataSource.Nhs)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
            await db.PublicSectorOrganisationEntities.AddRangeAsync(toAdd);
            db.PublicSectorOrganisationEntities.UpdateRange(toUpdate);

        });
    }
}

