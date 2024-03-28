using MediatR;

namespace SFA.DAS.PublicSectorOrganisations.Application.Queries.GetMatchingPublicSectorOrganisations;

public class GetMatchingPublicSectorOrganisationsQuery : IRequest<GetMatchingPublicSectorOrganisationsResponse>
{
    public string? SearchTerm { get; set; }
}