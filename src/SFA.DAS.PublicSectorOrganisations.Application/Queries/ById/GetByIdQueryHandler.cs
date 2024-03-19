using MediatR;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.ById;

public class GetByIdQueryHandler(IPublicSectorOrganisationRepository repository) : IRequestHandler<GetByIdQuery, GetByIdResponse?>
{
    public async Task<GetByIdResponse?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var organisation = await repository.GetPublicSectorOrganisationById(request.Id);
        if(organisation == null)
            return null;

        return new GetByIdResponse
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