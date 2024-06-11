using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Ons.OnsDownloadClient;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Net;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Ons.OnsDownloadService;

public class WhenCreatingExcelFileFromDownload
{
    [Test, MoqAutoData]
    public async Task And_finds_file_on_3rd_call_Then_saves_file_in_temp_location()
    {
        var config = new PublicSectorOrganisationsConfiguration
        {
            OnsUrl = "https://www.ons.gov.uk/file?/pscg{0}.xlsx",
            OnsUrlDateFormat = "MMMyyyy"
        };

        var filename = WhenDownloadingFile.GetFullPathForExcelFile();
        byte[] data = File.ReadAllBytes(filename);

        var getFileResponse = new HttpResponseMessage
        {
            Content = new ByteArrayContent(data),
            StatusCode = HttpStatusCode.OK
        };

        var getNotFoundResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        var testDate = new DateTime(2024, 03, 20);
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(testDate);
        var foundUrl = string.Format(config.OnsUrl, testDate.AddMonths(-2).ToString(config.OnsUrlDateFormat).ToLower());

        var onsDownloadClientMock = new Mock<IOnsDownloadClient>();
        onsDownloadClientMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(getNotFoundResponse);
        onsDownloadClientMock.Setup(x => x.GetAsync(foundUrl)).ReturnsAsync(getFileResponse);

        var sut = new Data.Ons.OnsDownloadService(onsDownloadClientMock.Object, dateTimeProviderMock.Object, config,
            Mock.Of<ILogger<Data.Ons.OnsDownloadService>>());

        var filePath = await sut.CreateLatestOnsExcelFile();
        filePath.EndsWith("publicsectorclassificationguidelatest.xlsx").Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task And_finds_no_file_Then_throws_exception()
    {
        var config = new PublicSectorOrganisationsConfiguration
        {
            OnsUrl = "https://www.ons.gov.uk/file?/pscg{0}.xlsx",
            OnsUrlDateFormat = "MMMyyyy"
        };

        var filename = WhenDownloadingFile.GetFullPathForExcelFile();
        byte[] data = File.ReadAllBytes(filename);

        var getFileResponse = new HttpResponseMessage
        {
            Content = new ByteArrayContent(data),
            StatusCode = HttpStatusCode.OK
        };

        var getNotFoundResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        var testDate = new DateTime(2024, 03, 20);
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(testDate);

        var onsDownloadClientMock = new Mock<IOnsDownloadClient>();
        onsDownloadClientMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(getNotFoundResponse);

        var sut = new Data.Ons.OnsDownloadService(onsDownloadClientMock.Object, dateTimeProviderMock.Object, config,
            Mock.Of<ILogger<Data.Ons.OnsDownloadService>>());

        var act = () => sut.CreateLatestOnsExcelFile();

        await act.Should().ThrowAsync<DownloadingExcelFileException>();
    }
}