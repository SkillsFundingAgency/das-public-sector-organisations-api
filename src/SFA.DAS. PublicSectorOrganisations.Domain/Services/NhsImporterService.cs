using System.Collections.Concurrent;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Services;

public interface INhsImporterService
{
    Task ImportData();
}

public class NhsImporterService : INhsImporterService
{
    private readonly INhsClient _client;

    public NhsImporterService(INhsClient client)
    {
        _client = client;
    }

    public async Task ImportData()
    {
        var summaryCalls = new List<NhsSector>
        {
            new() { ExternalCode = "ESPHA", InternalCode = "RO189"},
            new() { ExternalCode = "ECCG", InternalCode = "RO98"},
            new() { ExternalCode = "ECSU", InternalCode = "RO213"},
            new() { ExternalCode = "ECT", InternalCode = "RO107"},
            new() { ExternalCode = "ETR", InternalCode = "RO197"},
            new() { ExternalCode = "EOTHER-RO146", InternalCode = "RO146"},
            new() { ExternalCode = "EOTHER-RO147", InternalCode = "RO147"},
            new() { ExternalCode = "EOTHER-RO103", InternalCode = "RO103"},
            new() { ExternalCode = "EOTHER-RO191", InternalCode = "RO191"},
            new() { ExternalCode = "EOTHER-RO162", InternalCode = "RO162"},
            new() { ExternalCode = "EOTHER-RO116", InternalCode = "RO116"},
            new() { ExternalCode = "EOTHER-RO90", InternalCode = "RO90"},
            new() { ExternalCode = "EOTHER-RO91", InternalCode = "RO91"},
            new() { ExternalCode = "EOTHER-RO89", InternalCode = "RO89"},
            new() { ExternalCode = "EOTHER-RO131", InternalCode = "RO131"},
            new() { ExternalCode = "EOTHER-RO134", InternalCode = "RO134"},
            new() { ExternalCode = "EOTHER-RO105", InternalCode = "RO105"},
            new() { ExternalCode = "EOTHER-RO150", InternalCode = "RO150"},
            new() { ExternalCode = "EOTHER-RO102", InternalCode = "RO102"},
            new() { ExternalCode = "EOTHER-RO92", InternalCode = "RO92"},
            new() { ExternalCode = "EOTHER-RO159", InternalCode = "RO159"},
            new() { ExternalCode = "EOTHER-RO209", InternalCode = "RO209"},
            new() { ExternalCode = "EOTHER-RO210", InternalCode = "RO210"},
            new() { ExternalCode = "EOTHER-RO212", InternalCode = "RO212"},
        };

        var data = new ConcurrentBag<OrganisationSummary>();
        var records = new ConcurrentBag<Models.PublicSectorOrganisation>();

        await Parallel.ForEachAsync(summaryCalls, async (item, ct) =>
        {
            var response = await _client.GetAllOrganisations(item.InternalCode);

            foreach (var summary in response.Organisations)
            {
                data.Add(summary);
            }
        });

        await Parallel.ForEachAsync(data, async (item, ct) =>
        {
            var detail = await _client.GetOrganisation(item.OrgId);
            records.Add(new Models.PublicSectorOrganisation
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Source = DataSource.Nhs,
                AddressLine1 = detail.AddressLine1,
                AddressLine2 = detail.AddressLine2,
                AddressLine3 = detail.AddressLine3,
                Town = detail.Town,
                PostCode = detail.PostCode,
                Country = detail.Country,
                UPRN = detail.UPRN,
                OrganisationCode = item.OrgId,
            });
        });

        // Save to DB as a SqlTable insert

        // Update Real records by Merge Update Command and Update no matching record by marking them as inactive 
        var a = records.Count;
    }
}


public class NhsSector
{
    public string ExternalCode { get; set; }
    public string InternalCode { get; set; }
}