using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;
using SFA.DAS.PublicSectorOrganisations.Domain.Services;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Application.Commands.Import;

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