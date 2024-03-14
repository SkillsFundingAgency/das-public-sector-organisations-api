using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;

public class ImportCommandHandler : IRequestHandler<ImportCommand>
{
    private readonly INhsImporterService _nhsImporterService;
    private readonly IPoliceImporterService _policeImporterService;

    public ImportCommandHandler(INhsImporterService nhsImporterService, IPoliceImporterService policeImporterService)
    {
        _nhsImporterService = nhsImporterService;
        _policeImporterService = policeImporterService;
    }

    public async Task Handle(ImportCommand request, CancellationToken cancellationToken)
    {
        var importers = new List<Func<Task>>
        {
            () => _nhsImporterService.ImportData(),
            () => _policeImporterService.ImportData()
        };

        await Parallel.ForEachAsync(importers, cancellationToken, async (x, cancellationToken) =>
        {
            await x.Invoke();
        });
    }
}