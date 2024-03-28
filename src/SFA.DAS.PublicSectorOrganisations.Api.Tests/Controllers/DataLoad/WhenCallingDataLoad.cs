using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Api.Controllers;
using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Api.Tests.Controllers.DataLoad;
public class WhenCallingDataLoad
{
    [Test, MoqAutoData]
    public async Task Then_Gets_Result_From_Mediator(
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] DataLoadController controller)
    {
        var result = await controller.PostStart() as NoContentResult;

        result.Should().NotBeNull();

        mockMediator.Verify(x=>x.Send(It.IsAny<ImportCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}