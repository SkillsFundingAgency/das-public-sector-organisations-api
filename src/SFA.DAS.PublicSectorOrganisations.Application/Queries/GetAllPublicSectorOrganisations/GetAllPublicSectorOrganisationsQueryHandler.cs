using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetAllPublicSectorOrganisations;

public class GetAllPublicSectorOrganisationsQueryHandler(IPublicSectorOrganisationRepository repository) : IRequestHandler<GetAllPublicSectorOrganisationsQuery, GetAllPublicSectorOrganisationsResponse>
{
    public async Task<GetAllPublicSectorOrganisationsResponse> Handle(GetAllPublicSectorOrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = await repository.GetAllActivePublicSectorOrganisations();

        return new GetAllPublicSectorOrganisationsResponse(organisations.Select(organisation => new PublicSectorOrganisation
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