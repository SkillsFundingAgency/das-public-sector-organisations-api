using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Application.Tests.Commands.Import;
public class WhenCallingImportCommandHandler
{
    [Test, MoqAutoData]
    public async Task Then_NhsImporter_is_called(ImportCommand command, [Frozen] IEnumerable<Mock<IImporterService>> mockImporterServices)
    {
        var handler = new ImportCommandHandler(mockImporterServices.Select(x => x.Object));
        await handler.Handle(command, CancellationToken.None);

        foreach (var importer in mockImporterServices)
        {
            importer.Verify(x=>x.ImportData(), Times.Once);
        }
    }
}