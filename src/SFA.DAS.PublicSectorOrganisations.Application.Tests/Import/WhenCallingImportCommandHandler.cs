using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Application.Tests.Import
{
    public class WhenCallingImportCommandHandler
    {
        [Test, MoqAutoData]
        public async Task Then_NhsImporter_is_called(
            ImportCommand command,
            [Frozen] Mock<INhsImporterService> nhsImporter,
            [Greedy] ImportCommandHandler handler
            )
        {
            await handler.Handle(command, CancellationToken.None);

            nhsImporter.Verify(x=>x.ImportData(), Times.Once);
        }
    }
}
