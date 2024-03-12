using AutoFixture.NUnit3;
using Moq.Protected;
using Moq;
using NUnit.Framework;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.PublicSectorOrganisations.Data.Nhs;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Helpers;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Nhs.NhsClient;
public class WhenCallingGetOrganisation
{
    [Test, AutoData]
    public async Task Then_GetOrganisation_is_called_correctly_and_returns_expected_response(
        GetOrganisationResponse responseFromApi)
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
        var sut = new Data.Nhs.NhsClient(httpClient);
        var expectedResponse = new GetSingleOrganisationResponse()
        {
            AddressLine1 = responseFromApi.Organisation.GeoLoc.Location.AddrLn1,
            AddressLine2 = responseFromApi.Organisation.GeoLoc.Location.AddrLn2,
            AddressLine3 = responseFromApi.Organisation.GeoLoc.Location.AddrLn3,
            Town = responseFromApi.Organisation.GeoLoc.Location.Town,
            Country = responseFromApi.Organisation.GeoLoc.Location.Country,
            PostCode = responseFromApi.Organisation.GeoLoc.Location.PostCode,
            UPRN = responseFromApi.Organisation.GeoLoc.Location.UPRN
        };
        //Act
        var response = await sut.GetOrganisation(sector);

        //Assert
        response.Should().BeEquivalentTo(expectedResponse);

        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.Equals($"https://nhs.org/ORD/2-0-0/organisations/{sector}", StringComparison.InvariantCultureIgnoreCase)
                    ),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}