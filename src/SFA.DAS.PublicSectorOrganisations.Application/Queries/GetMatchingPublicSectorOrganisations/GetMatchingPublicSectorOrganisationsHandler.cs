using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetMatchingPublicSectorOrganisations;

public class GetMatchingPublicSectorOrganisationsHandler(IPublicSectorOrganisationRepository repository) : IRequestHandler<GetMatchingPublicSectorOrganisationsQuery, GetMatchingPublicSectorOrganisationsResponse>
{
    public async Task<GetMatchingPublicSectorOrganisationsResponse> Handle(
        GetMatchingPublicSectorOrganisationsQuery request, CancellationToken cancellationToken)
    {
        List<PublicSectorOrganisationEntity> organisations;

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            organisations = await repository.GetAllActivePublicSectorOrganisations();
        }
        else
        {
            organisations = await repository.GetMatchingActivePublicSectorOrganisations(request.SearchTerm);
        }
        
        return new GetMatchingPublicSectorOrganisationsResponse(organisations.Select(organisation => new PublicSectorOrganisation
        {
            Id = organisation.Id.Value,
            Name = organisation.Name,
            Source = organisation.Source.ToString(),
            AddressLine1 = organisation.AddressLine1,
            AddressLine2 = organisation.AddressLine2,
            AddressLine3 = organisation.AddressLine3,
            Town = organisation.Town,
            PostCode = organisation.PostCode,
            Country = organisation.Country,
            UPRN = organisation.UPRN,
            OrganisationCode = organisation.OrganisationCode,
            OnsSector = organisation.OnsSector,
            Active = organisation.Active
        }).ToArray());

    }
}