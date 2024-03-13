using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data;

public class PublicSectorOrganisationRepository : IPublicSectorOrganisationRepository
{
    private readonly ILogger<PublicSectorOrganisationRepository> _logger;
    private PublicSectorOrganisationDataContext _db;

    public PublicSectorOrganisationRepository(Lazy<PublicSectorOrganisationDataContext> dbContext, ILogger<PublicSectorOrganisationRepository> logger)
    {
        _logger = logger;
        _db = dbContext.Value;
    }
    public Task<List<PublicSectorOrganisationEntity>> GetPublicSectorOrganisationsFor(DataSource dataSource)
    {
        return _db.PublicSectorOrganisationEntities.Where(x => x.Source == dataSource).AsNoTracking().ToListAsync();
    }

    public async Task UpdateAndAddPublicSectorOrganisationsFor(DataSource dataSource, ConcurrentBag<PublicSectorOrganisationEntity> toUpdate, ConcurrentBag<PublicSectorOrganisationEntity> toAdd)
    {
        await _db.ExecuteInATransaction(async () =>
        {
            _logger.LogInformation("Updating {existingCount} and adding {newCount} NHS Organisations", toUpdate.Count, toAdd.Count);
            await _db.PublicSectorOrganisationEntities.Where(x => x.Source == DataSource.Nhs)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
            await _db.PublicSectorOrganisationEntities.AddRangeAsync(toAdd);
            _db.PublicSectorOrganisationEntities.UpdateRange(toUpdate);

        });
    }
}

