using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;
using ClosedXML.Excel;

namespace SFA.DAS.PublicSectorOrganisations.Data.Ons
{
    public class OnsExcelReaderServiceUsingClosedXml(ILogger<OnsExcelReaderServiceUsingClosedXml> logger) : IOnsExcelReaderService
    {
        public List<OnsExcelDetail> GetOnsDataFromSpreadsheet(string excelFile)
        {
            const string workbookName = "Organisation|Institutional Unit";
            
            try
            {
                var records = new List<OnsExcelDetail>();
                using var workbook = new XLWorkbook(excelFile);

                var workSheet = workbook.Worksheet("Organisation|Institutional Unit");

                CheckWorksheetLayout(workSheet);

                var rows = workSheet.RangeUsed().RowsUsed().Skip(6);

                foreach (var row in rows)
                {
                    var name = row.Cell(1).GetValue<string>();
                    var sector = row.Cell(2).GetValue<string>();
                    var esa = row.Cell(3).GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        records.Add(new OnsExcelDetail
                                {
                                    Name = name,
                                    Sector = sector,
                                    EsaCode = esa,
                                });
                    }
                }
                return records;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot get ONS organisations, potential format change");
                throw new ReadingOnsExcelFileException("Problem getting ONS organisations", e);
            }
        }

        private static void CheckWorksheetLayout(IXLWorksheet workSheet)
        {
            var titleRow = workSheet.Row(6);
            var nameTitle = titleRow.Cell(1).GetValue<string>();
            var sectorTitle = titleRow.Cell(2).GetValue<string>();
            var esaTitle = titleRow.Cell(3).GetValue<string>();

            if (nameTitle == null || !nameTitle.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Expected column title 'Name' not present");
            }
            if (sectorTitle == null || !sectorTitle.Equals("Sector Classification", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Expected column title 'Sector Classification' not present");
            }
            if (esaTitle == null || esaTitle.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Expected column title 'Name' not present");
            }
        }
    }
}
