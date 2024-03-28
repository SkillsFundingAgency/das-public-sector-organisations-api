using System.Net;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Api.Controllers;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetMatchingPublicSectorOrganisations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Api.Tests.Controllers.PublicSectorOrganisations;
public class WhenCallingGetMatchingPublicSectorOrganisations
{
    [Test, MoqAutoData]
    public async Task And_when_none_found_Then_controller_returns_empty_list()
    {
        var response = new GetMatchingPublicSectorOrganisationsResponse(Array.Empty<PublicSectorOrganisation>());
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(It.IsAny<GetMatchingPublicSectorOrganisationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = new PublicSectorOrganisationsController(mediatorMock.Object);
        var result = await sut.GetMatches("Xyz") as OkObjectResult;

        result.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var myApiResponse = result.Value as GetMatchingPublicSectorOrganisationsResponse;
        myApiResponse.PublicSectorOrganisations.Length.Should().Be(0);
        mediatorMock.Verify(x=>x.Send(It.Is<GetMatchingPublicSectorOrganisationsQuery>(p=>p.SearchTerm == "Xyz"), It.IsAny<CancellationToken>()));
    }

    [Test, MoqAutoData]
    public async Task And_when_records_exists_Then_controller_returns_Ok_and_result(GetMatchingPublicSectorOrganisationsResponse response)
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(It.IsAny<GetMatchingPublicSectorOrganisationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = new PublicSectorOrganisationsController(mediatorMock.Object);
        var result = await sut.GetMatches(null) as OkObjectResult;

        var myApiResponse = result.Value as GetMatchingPublicSectorOrganisationsResponse;

        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeEquivalentTo(response);
        mediatorMock.Verify(x => x.Send(It.Is<GetMatchingPublicSectorOrganisationsQuery>(p => p.SearchTerm == null), It.IsAny<CancellationToken>()));
    }
}