using System.Collections.Concurrent;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.Nhs;

public class NhsImporterService : INhsImporterService
{
    private readonly INhsClient _client;
    private readonly Lazy<PublicSectorOrganisationDataContext> _context;
    private readonly ILogger<NhsImporterService> _logger;
    private readonly NhsSector[] _sectors;

    public NhsImporterService(INhsClient client, PublicSectorOrganisationsConfiguration publicSectorOrganisationsConfiguration, Lazy<PublicSectorOrganisationDataContext> dbContext, ILogger<NhsImporterService> logger)
    {
        _client = client;
        _context = dbContext;
        _logger = logger;
        _sectors = publicSectorOrganisationsConfiguration.NhsSectors;
    }

    public async Task ImportData()
    {
        var data = new ConcurrentBag<OrganisationSummary>();

        await Parallel.ForEachAsync(_sectors, async (item, ct) =>
        {
            var response = await _client.GetAllOrganisations(item.InternalCode);

            foreach (var summary in response.Organisations)
            {
                data.Add(summary);
            }
        });

        var newRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();
        var updateRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();

        var db = _context.Value;
        var nhsList = await db.PublicSectorOrganisationEntities.Where(x=>x.Source == DataSource.Nhs).AsNoTracking().ToListAsync();

        await Parallel.ForEachAsync(data, async (item, ct) =>
        {
            var detail = await _client.GetOrganisation(item.OrgId);
            var existingEntity = nhsList.FirstOrDefault(x =>
                x.OrganisationCode.Equals(item.OrgId, StringComparison.InvariantCultureIgnoreCase));

            var record = new PublicSectorOrganisationEntity
            {
                Id = existingEntity == null ? Guid.NewGuid() : existingEntity.Id,
                Name = item.Name,
                Source = DataSource.Nhs,
                AddressLine1 = detail.AddressLine1,
                AddressLine2 = detail.AddressLine2,
                AddressLine3 = detail.AddressLine3,
                Town = detail.Town,
                PostCode = detail.PostCode,
                Country = detail.Country,
                UPRN = detail.UPRN,
                OrganisationCode = item.OrgId,
                Active = true
            };

            if (existingEntity != null)
            {
                updateRecords.Add(record);
            }
            else
            {
                newRecords.Add(record);    
            }
        });

        try
        {
            await db.Database.BeginTransactionAsync();
            await db.PublicSectorOrganisationEntities.Where(x=>x.Source == DataSource.Nhs).ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
            await db.PublicSectorOrganisationEntities.AddRangeAsync(newRecords);
            db.PublicSectorOrganisationEntities.UpdateRange(updateRecords);
            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();
        }
        catch
        {
            await db.Database.RollbackTransactionAsync();
            throw;
        }
    }
}


