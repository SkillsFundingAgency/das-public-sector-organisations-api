using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Import;

public class ImportCommandHandler : IRequestHandler<ImportCommand>
{
    private readonly INhsImporterService _nhsImporterService;

    public ImportCommandHandler(INhsImporterService nhsImporterService)
    {
        _nhsImporterService = nhsImporterService;
    }

    public async Task Handle(ImportCommand request, CancellationToken cancellationToken)
    {
        await _nhsImporterService.ImportData();

    }
}