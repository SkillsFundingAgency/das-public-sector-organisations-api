using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Api.Controllers;
using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.ById;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Api.Tests.Controllers.PublicSectorOrganisations;
public class WhenCallingGetPublicSectorOrganisationById
{
    [Test, MoqAutoData]
    public async Task And_when_no_match_found_Then_controller_returns_NotFound(
        Guid id,
        PublicSectorOrganisationsController controller)
    {
        var mediatorMock = new Mock<IMediator>();

        var sut = new PublicSectorOrganisationsController(mediatorMock.Object);
        var result = await sut.GetById(id) as NotFoundResult;

        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Test, MoqAutoData]
    public async Task And_match_found_then_controller_returns_Ok_and_result(
        GetByIdResponse response)
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(It.Is<GetByIdQuery>(p => p.Id == response.Id), It.IsAny<CancellationToken>())).ReturnsAsync(response);


        var sut = new PublicSectorOrganisationsController(mediatorMock.Object);
        var result = await sut.GetById(response.Id) as OkObjectResult;

        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeEquivalentTo(response);
    }

}