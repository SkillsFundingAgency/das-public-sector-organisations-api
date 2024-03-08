using AutoFixture.NUnit3;
using Moq.Protected;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.PublicSectorOrganisations.Data.Nhs;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Helpers;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Nhs;
public class WhenCallingGetAllOrganisations
{
    [Test, AutoData]
    public async Task Then_the_GetAllOrganisations_is_called_correctly_and_returns_expected_response(
        GetAllOrganisationsResponse responseFromApi)
    {
        var sector = "Test";
        var baseUrl = "https://nhs.org";
        var getResponse = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(responseFromApi)),
            StatusCode = HttpStatusCode.OK
        };
        var httpMessageHandler = MessageHandlerHelper.SetupMessageHandlerMock(getResponse, baseUrl, "GET");
        var httpClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        var sut = new NhsClient(httpClient);

        //Act
        var response = await sut.GetAllOrganisations(sector);

        //Assert
        response.Should().BeEquivalentTo(responseFromApi);

        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.StartsWith("https://nhs.org/ORD/2-0-0/organisations", StringComparison.InvariantCultureIgnoreCase)
                    && c.RequestUri.AbsoluteUri.EndsWith($"?Limit=1000&Status=Active&PrimaryRoleId={sector}", StringComparison.InvariantCultureIgnoreCase)
                    ),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}