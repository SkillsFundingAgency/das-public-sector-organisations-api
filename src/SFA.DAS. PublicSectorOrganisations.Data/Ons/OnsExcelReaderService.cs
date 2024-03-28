using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;
using System.Data.OleDb;
using System.Data;
using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

namespace SFA.DAS.PublicSectorOrganisations.Data.Ons
{
    public class OnsExcelReaderService(ILogger<OnsExcelReaderService> logger) : IOnsExcelReaderService
    {
        public List<OnsExcelDetail> GetOnsDataFromSpreadsheet(string excelFile)
        {
            const string sheetName = "Organisation|Institutional Unit$";
            var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties='Excel 12.0 Xml;HDR=YES;';Data Source={excelFile}";

            try
            {
                using var conn = new OleDbConnection(connectionString);
                using var cmd = new OleDbCommand();
                
                conn.Open();
                cmd.Connection = conn;

                conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                cmd.CommandText = "SELECT [Name], [Sector Classification],[ESA 2010 Code] FROM [" + sheetName +
                                  "A6:C] WHERE [Name] IS NOT NULL "; 

                var dt = new DataTable(sheetName);
                var da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                var data = dt.AsEnumerable();

                var records = data.Select(
                    x =>
                        new OnsExcelDetail
                        {
                            Name = x.Field<string>("Name"),
                            Sector = x.Field<string>("Sector Classification"),
                            EsaCode = x.Field<string>("ESA 2010 Code"),
                        }).ToList();
                conn.Close();
                return records;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot get ONS organisations, potential format change");
                throw new ReadingOnsExcelFileException("Problem getting ONS organisations", e);
            }
        }
    }
}
