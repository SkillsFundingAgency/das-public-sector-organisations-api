using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Helpers;
using SFA.DAS.PublicSectorOrganisations.Domain.NhsClientResponse;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Police.PoliceApiClient;
public class WhenCallingGetAllPoliceForces
{
    [Test, AutoData]
    public async Task Then_the_GetAllOrganisations_is_called_correctly_and_returns_expected_response(
        PoliceForce[] responseFromApi)
    {
        var baseUrl = "https://police.org.uk/forces";
        var getResponse = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(responseFromApi)),
            StatusCode = HttpStatusCode.OK
        };
        var httpMessageHandler = MessageHandlerHelper.SetupMessageHandlerMock(getResponse, "GET");
        var httpClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        var sut = new Data.Police.PoliceApiClient(httpClient);

        //Act
        var response = await sut.GetAllPoliceForces();

        //Assert
        response.Should().BeEquivalentTo(responseFromApi);

        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.Equals(baseUrl, StringComparison.InvariantCultureIgnoreCase)
                    ),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}