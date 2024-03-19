using MediatR;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetAllPublicSectorOrganisations;

public class GetAllPublicSectorOrganisationsQuery : IRequest<GetAllPublicSectorOrganisationsResponse>
{
    public Guid Id { get; set; }
}