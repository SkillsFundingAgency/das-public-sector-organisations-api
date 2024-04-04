using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IOnsExcelReaderService
{
    List<OnsExcelDetail> GetOnsDataFromSpreadsheet(string excelFile);
}