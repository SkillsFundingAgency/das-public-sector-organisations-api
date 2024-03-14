using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.Police;

public class PoliceImporterService : IPoliceImporterService

{
    private readonly IPoliceApiClient _client;
    private readonly IPublicSectorOrganisationRepository _dbRepository;
    private readonly ILogger<PoliceImporterService> _logger;

    public PoliceImporterService(IPoliceApiClient client,
        IPublicSectorOrganisationRepository dbRepository,
        ILogger<PoliceImporterService> logger)
    {
        _client = client;
        _dbRepository = dbRepository;
        _logger = logger;
    }

    public async Task ImportData()
    {
        var newRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();
        var updateRecords = new ConcurrentBag<PublicSectorOrganisationEntity>();

        var nhsList = await _dbRepository.GetPublicSectorOrganisationsFor(DataSource.Police);

        await FetchNewAndExistingDetails(nhsList, updateRecords, newRecords);

        await _dbRepository.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Police, updateRecords, newRecords);
    }

    private async Task FetchNewAndExistingDetails(IReadOnlyCollection<PublicSectorOrganisationEntity> nhsList, ConcurrentBag<PublicSectorOrganisationEntity> updateRecords,
        ConcurrentBag<PublicSectorOrganisationEntity> newRecords)
    {
        try
        {
            _logger.LogInformation("Collecting Police force details");

            var data= await _client.GetAllPoliceForces();

            foreach (var item in data)
            {

                var existingEntity = nhsList.FirstOrDefault(x =>
                    x.OrganisationCode.Equals(item.Id, StringComparison.InvariantCultureIgnoreCase));

                var record = new PublicSectorOrganisationEntity
                {
                    Id = existingEntity == null ? Guid.NewGuid() : existingEntity.Id,
                    Name = item.Name,
                    Source = DataSource.Police,
                    OrganisationCode = item.Id,
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
            }

            _logger.LogInformation("Completed collecting Police force details");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Collecting Police force details failed with : {message}", e.Message);
            throw;
        }
    }


}