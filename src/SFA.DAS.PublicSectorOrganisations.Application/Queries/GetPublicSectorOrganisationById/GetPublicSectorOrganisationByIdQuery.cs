using MediatR;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetPublicSectorOrganisationById;

public class GetPublicSectorOrganisationByIdQuery : IRequest<GetPublicSectorOrganisationByIdResponse?>
{
    public Guid Id { get; set; }
}