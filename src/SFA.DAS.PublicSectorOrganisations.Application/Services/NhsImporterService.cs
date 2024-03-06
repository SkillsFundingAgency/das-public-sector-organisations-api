using System.Collections.Concurrent;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;

namespace SFA.DAS.PublicSectorOrganisations.Application.Services;

public class NhsImporterService : INhsImporterService
{
    private readonly INhsClient _client;
    private readonly List<NhsSector> _sectors;

    public NhsImporterService(INhsClient client, PublicSectorOrganisationsConfiguration publicSectorOrganisationsConfiguration)
    {
        _client = client;
        _sectors = publicSectorOrganisationsConfiguration.NhsSectors.ToList();
    }

    public async Task ImportData()
    {
        var data = new ConcurrentBag<OrganisationSummary>();
        var records = new ConcurrentBag<Domain.Models.PublicSectorOrganisation>();

        await Parallel.ForEachAsync(_sectors, async (item, ct) =>
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
            records.Add(new Domain.Models.PublicSectorOrganisation
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


