using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Data.Ons;
public class OnsImporterService : IImporterService
{
    private readonly IOnsDownloadService _onsDownloadService;
    private readonly IOnsExcelReaderService _onsExcelReaderService;
    private readonly IPublicSectorOrganisationRepository _publicSectorOrganisationRepository;
    private readonly ILogger<OnsImporterService> _logger;

    public OnsImporterService(IOnsDownloadService onsDownloadService,
        IOnsExcelReaderService onsExcelReaderService,
        IPublicSectorOrganisationRepository publicSectorOrganisationRepository,

        ILogger<OnsImporterService> logger)
    {
        _onsDownloadService = onsDownloadService;
        _onsExcelReaderService = onsExcelReaderService;
        _publicSectorOrganisationRepository = publicSectorOrganisationRepository;
        _logger = logger;
    }

    public async Task ImportData()
    {
        var newRecords = new List<PublicSectorOrganisationEntity>();
        var updateRecords = new List<PublicSectorOrganisationEntity>();


        var filename = await _onsDownloadService.CreateLatestOnsExcelFile();
        var importedOnsList = _onsExcelReaderService.GetOnsDataFromSpreadsheet(filename);

        await CreateNewAndExistingDetailsFromImportedList(importedOnsList, updateRecords, newRecords);

        await _publicSectorOrganisationRepository.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons, updateRecords, newRecords);
    }

    private async Task CreateNewAndExistingDetailsFromImportedList(List<OnsExcelDetail> importedOnsList, List<PublicSectorOrganisationEntity> updateRecords, List<PublicSectorOrganisationEntity> newRecords)
    {

        _logger.LogInformation("Sorting ONS Details");
        var onsList = await _publicSectorOrganisationRepository.GetPublicSectorOrganisationsForDataSource(DataSource.Ons);

        foreach (var item in importedOnsList.Where(x =>
                     x.EsaCode != null && !x.EsaCode.Equals("Disbanded or Deleted Entity",
                         StringComparison.InvariantCultureIgnoreCase)))
        {
            var existingEntity = onsList.FirstOrDefault(x =>
                x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && 
                x.OnsSector != null && x.OnsSector.Equals(item.Sector, StringComparison.InvariantCultureIgnoreCase));

            if (existingEntity != null)
            {
                existingEntity.Name = item.Name;
                existingEntity.OnsSector = item.Sector;
                existingEntity.Active = true;
                updateRecords.Add(existingEntity);
            }
            else
            {
                newRecords.Add(new PublicSectorOrganisationEntity
                {
                    Id = existingEntity == null ? Guid.NewGuid() : existingEntity.Id,
                    Name = item.Name,
                    Source = DataSource.Ons,
                    OnsSector = item.Sector,
                    Active = true
                });
            }
        }
        _logger.LogInformation("Completed sorting ONS Details");
    }
}
