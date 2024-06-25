using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetPublicSectorOrganisationById;

public class GetPublicSectorOrganisationByIdQueryHandler(IPublicSectorOrganisationRepository repository) : IRequestHandler<GetPublicSectorOrganisationByIdQuery, GetPublicSectorOrganisationByIdResponse?>
{
    public async Task<GetPublicSectorOrganisationByIdResponse?> Handle(GetPublicSectorOrganisationByIdQuery request, CancellationToken cancellationToken)
    {
        var organisation = await repository.GetPublicSectorOrganisationById(request.Id);
        if(organisation == null)
            return null;

        return new GetPublicSectorOrganisationByIdResponse
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
        };
    }
}