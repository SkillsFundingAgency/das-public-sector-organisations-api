﻿using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Data.Tests.Helpers;
using SFA.DAS.PublicSectorOrganisations.Domain.NhsClientResponse;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Nhs.NhsApiClient;
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
        var httpMessageHandler = MessageHandlerHelper.SetupMessageHandlerMock(getResponse, "GET");
        var httpClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        var sut = new Data.Nhs.NhsApiClient(httpClient);

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