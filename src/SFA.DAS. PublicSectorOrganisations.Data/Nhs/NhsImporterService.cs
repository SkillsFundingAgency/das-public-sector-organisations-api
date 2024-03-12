using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.PublicSectorOrganisations.Data.Extensions;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.Nhs;

public class NhsImporterService : INhsImporterService
{
    private readonly INhsClient _client;
    private readonly IPublicSectorOrganisationDataContext _dbContext;
    private readonly ILogger<NhsImporterService> _logger;
    private readonly NhsSector[] _sectors;

    public int NumberOfRecordsAdded { get; private set; }
    public int NumberOfRecordsUpdated { get; private set; }

    public NhsImporterService(INhsClient client,
        PublicSectorOrganisationsConfiguration publicSectorOrganisationsConfiguration,
        IPublicSectorOrganisationDataContext dbContext,
        ILogger<NhsImporterService> logger)
    {
        _client = client;
        _dbContext = dbContext;
        _logger = logger;
        _sectors = publicSectorOrganisationsConfiguration.NhsSectors;
    }

    public async Task ImportData()
    {
        var data = await FetchSectorSummaries();

        var newRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();
        var updateRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();

        var nhsList = _dbContext.PublicSectorOrganisationEntities.Where(x=>x.Source == DataSource.Nhs).AsNoTracking().ToList();

        await FetchNewAndExistingDetails(data, nhsList, updateRecords, newRecords);
        NumberOfRecordsAdded = newRecords.Count;
        NumberOfRecordsUpdated = updateRecords.Count;

        await _dbContext.ExecuteInATransaction(async () =>
        {
            _logger.LogInformation("Updating {existingCount} and adding {newCount} NHS Organisations", updateRecords.Count, newRecords.Count);
            await _dbContext.PublicSectorOrganisationEntities.Where(x => x.Source == DataSource.Nhs)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
            await _dbContext.PublicSectorOrganisationEntities.AddRangeAsync(newRecords);
            _dbContext.PublicSectorOrganisationEntities.UpdateRange((updateRecords));

        });
    }

    private async Task FetchNewAndExistingDetails(ConcurrentBag<OrganisationSummary> data, IReadOnlyCollection<PublicSectorOrganisationEntity> nhsList, ConcurrentBag<PublicSectorOrganisationEntity> updateRecords,
        ConcurrentBag<PublicSectorOrganisationEntity> newRecords)
    {
        try
        {
            _logger.LogInformation("Collecting NHS Details for each Organisation");
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
            _logger.LogInformation("Completed collecting NHS Details for each Organisation");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Collecting NHS Details for each Organisation failed with : {message}", e.Message);
            throw;
        }
    }

    private async Task<ConcurrentBag<OrganisationSummary>> FetchSectorSummaries()
    {
        var data = new ConcurrentBag<OrganisationSummary>();
        try
        {
            _logger.LogInformation("Collecting NHS Organisations by Sector");
            await Parallel.ForEachAsync(_sectors, async (item, ct) =>
            {
                var response = await _client.GetAllOrganisations(item.InternalCode);

                foreach (var summary in response.Organisations)
                {
                    data.Add(summary);
                }
            });
            _logger.LogInformation("Completed collecting NHS Organisations by Sector");

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Collecting NHS Organisations by Sector failed with : {message}", e.Message);
            throw;
        }

        return data;
    }
}