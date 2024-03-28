using System.Net;
using System.Reflection;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Helpers;
using SFA.DAS.PublicSectorOrganisations.Domain.NhsClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Ons.OnsDownloadClient;

public class WhenDownloadingFile
{
    [Test, AutoData]
    public async Task And_its_not_there_we_returns_NotFound_response(
        GetAllOrganisationsResponse responseFromApi)
    {
        var sector = "Test";
        var baseUrl = "https://ons.org?file=ABC122024.xls";
        var getResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };
        var httpMessageHandler = MessageHandlerHelper.SetupMessageHandlerMock(getResponse, "GET");
        var httpClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        var sut = new Data.Ons.OnsDownloadClient(httpClient);

        //Act
        var response = await sut.GetAsync(sector);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    ),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Test, AutoData]
    public async Task And_its_found_we_return_OK_response_With_fileContents(
        GetAllOrganisationsResponse responseFromApi)
    {
        var sector = "Test";
        var fileUrl = "https://ons.org?file=ABC122024.xls";

        var filename = GetFullPathForExcelFile();
        byte[] data = File.ReadAllBytes(filename);

        var getResponse = new HttpResponseMessage
        {
            Content = new ByteArrayContent(data),
            StatusCode = HttpStatusCode.OK
        };
        var httpMessageHandler = MessageHandlerHelper.SetupMessageHandlerMock(getResponse, "GET");
        var httpClient = new HttpClient(httpMessageHandler.Object);
        var sut = new Data.Ons.OnsDownloadClient(httpClient);

        //Act
        var response = await sut.GetAsync(fileUrl);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsByteArrayAsync();
        content.Should().BeEquivalentTo(data);

        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                   && c.RequestUri.AbsoluteUri.StartsWith("https://ons.org", StringComparison.InvariantCultureIgnoreCase)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    public static string GetFullPathForExcelFile()
    {
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string subfolderPath = Path.Combine(assemblyDirectory, "Data");
        return Path.Combine(subfolderPath, "pscgjan2024Test.xlsx");
    }
}