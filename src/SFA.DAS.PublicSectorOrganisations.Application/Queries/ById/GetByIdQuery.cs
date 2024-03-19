using MediatR;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.ById;

public class GetByIdQuery : IRequest<GetByIdResponse?>
{
    public Guid Id { get; set; }
}