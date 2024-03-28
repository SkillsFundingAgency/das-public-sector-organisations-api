using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;

public class ImportCommandHandler(IEnumerable<IImporterService> importers) : IRequestHandler<ImportCommand>
{
    public async Task Handle(ImportCommand request, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(importers, cancellationToken, async (x, cancellationToken) =>
        {
            await x.ImportData();
        });
    }
}