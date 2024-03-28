using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Ons.OnsImporterService;

public class WhenCreatingExcelFileFromDownload
{
    [Test, MoqAutoData]
    public async Task Then_calls_apis_and_saves_new_records_in_database(
        string filename,
        List<OnsExcelDetail> excelData,
        ILogger<Data.Ons.OnsImporterService> logger
        )
    {
        var onsDownloadServiceMock = new Mock<IOnsDownloadService>();
        onsDownloadServiceMock.Setup(x => x.CreateLatestOnsExcelFile()).ReturnsAsync(filename);

        var onsExcelReaderServiceMock = new Mock<IOnsExcelReaderService>();
        onsExcelReaderServiceMock.Setup(x => x.GetOnsDataFromSpreadsheet(filename)).Returns(excelData);

        var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
        dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsForDataSource(DataSource.Ons)).ReturnsAsync(new List<PublicSectorOrganisationEntity>());

        var sut = new Data.Ons.OnsImporterService(onsDownloadServiceMock.Object, onsExcelReaderServiceMock.Object, dbRepositoryMock.Object, logger);
        await sut.ImportData();

        dbRepositoryMock.Verify(x=>x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons, 
            It.Is<List<PublicSectorOrganisationEntity>>(p=>p.Count == 0), It.IsAny<List<PublicSectorOrganisationEntity>>()));

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons, It.IsAny<List<PublicSectorOrganisationEntity>>(),
            It.Is<List<PublicSectorOrganisationEntity>>(p => p.Count == 3 &&
                                                                      VerifyMappedRecordHasExpectedValues(p, excelData[0]) &&
                                                                      VerifyMappedRecordHasExpectedValues(p, excelData[1]) &&
                                                                      VerifyMappedRecordHasExpectedValues(p, excelData[2]))));
    }

    [Test, MoqAutoData]
    public async Task Then_calls_apis_and_adds_new_records_and_saves_updated_records_in_database(
        string filename,
        List<OnsExcelDetail> excelData,
        ILogger<Data.Ons.OnsImporterService> logger
        )
    {

        var onsDownloadServiceMock = new Mock<IOnsDownloadService>();
        onsDownloadServiceMock.Setup(x => x.CreateLatestOnsExcelFile()).ReturnsAsync(filename);

        var onsExcelReaderServiceMock = new Mock<IOnsExcelReaderService>();
        onsExcelReaderServiceMock.Setup(x => x.GetOnsDataFromSpreadsheet(filename)).Returns(excelData);

        var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
        dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsForDataSource(DataSource.Ons))
            .ReturnsAsync(new List<PublicSectorOrganisationEntity>
            {
                new PublicSectorOrganisationEntity
                {
                    Name = excelData[0].Name,
                    OnsSector = excelData[0].Sector,
                    Source = DataSource.Ons
                }
            });

        var sut = new Data.Ons.OnsImporterService(onsDownloadServiceMock.Object, onsExcelReaderServiceMock.Object, dbRepositoryMock.Object, logger);
        await sut.ImportData();

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons,
            It.Is<List<PublicSectorOrganisationEntity>>(p => p.Count == 1),
            It.Is<List<PublicSectorOrganisationEntity>>(p => p.Count == 2)));

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons,
            It.Is<List<PublicSectorOrganisationEntity>>(p =>
                VerifyMappedRecordHasExpectedValues(p, excelData[0])),
            It.Is<List<PublicSectorOrganisationEntity>>(p =>
                VerifyMappedRecordHasExpectedValues(p, excelData[1]) &&
                VerifyMappedRecordHasExpectedValues(p, excelData[2]))));
    }

    [Test, MoqAutoData]
    public async Task Then_finds_no_records_to_add_or_update_when_source_data_is_DisbandedOrDeleted(
        string filename,
        ILogger<Data.Ons.OnsImporterService> logger
        )
    {

        var f = new AutoFixture.Fixture();
        var excelData = f.Build<OnsExcelDetail>().With(x => x.EsaCode, "Disbanded or Deleted Entity").Create();

        var onsDownloadServiceMock = new Mock<IOnsDownloadService>();
        onsDownloadServiceMock.Setup(x => x.CreateLatestOnsExcelFile()).ReturnsAsync(filename);

        var onsExcelReaderServiceMock = new Mock<IOnsExcelReaderService>();
        onsExcelReaderServiceMock.Setup(x => x.GetOnsDataFromSpreadsheet(filename)).Returns(new List<OnsExcelDetail> { excelData });

        var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
        dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsForDataSource(DataSource.Ons)).ReturnsAsync(new List<PublicSectorOrganisationEntity>());

        var sut = new Data.Ons.OnsImporterService(onsDownloadServiceMock.Object, onsExcelReaderServiceMock.Object, dbRepositoryMock.Object, logger);
        await sut.ImportData();

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Ons,
            It.Is<List<PublicSectorOrganisationEntity>>(p => p.Count == 0),
            It.Is<List<PublicSectorOrganisationEntity>>(p => p.Count == 0)));

    }

    private bool VerifyMappedRecordHasExpectedValues(List<PublicSectorOrganisationEntity> records, OnsExcelDetail detail)
    {
        records.First(x => x.Name == detail.Name && x.OnsSector == detail.Sector).Should().BeEquivalentTo(
            new 
            {
                detail.Name,
                OnsSector = detail.Sector,
                Source = DataSource.Ons
            });
        return true;
    }
}