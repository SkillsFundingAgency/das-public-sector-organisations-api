using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Commands.Import;

public class ImportCommandHandler : IRequestHandler<ImportCommand>
{
    private readonly INhsImporterService _nhsImporterService;
    private readonly IPoliceImporterService _policeImporterService;
    private readonly IOnsImporterService _onsImporterService;

    public ImportCommandHandler(INhsImporterService nhsImporterService, IPoliceImporterService policeImporterService, IOnsImporterService onsImporterService)
    {
        _nhsImporterService = nhsImporterService;
        _policeImporterService = policeImporterService;
        _onsImporterService = onsImporterService;
    }

    public async Task Handle(ImportCommand request, CancellationToken cancellationToken)
    {
        var importers = new List<Func<Task>>
        {
            () => _nhsImporterService.ImportData(),
            () => _policeImporterService.ImportData(),
            () => _onsImporterService.ImportData()
        };

        await Parallel.ForEachAsync(importers, cancellationToken, async (x, cancellationToken) =>
        {
            await x.Invoke();
        });
    }
}