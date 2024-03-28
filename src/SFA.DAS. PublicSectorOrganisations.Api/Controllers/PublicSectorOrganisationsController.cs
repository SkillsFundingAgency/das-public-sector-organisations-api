using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetMatchingPublicSectorOrganisations;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetPublicSectorOrganisationById;

namespace SFA.DAS.PublicSectorOrganisations.Api.Controllers
{
    [ApiVersion("1.0")]
    [Controller]
    [Route("/PublicSectorOrganisations")]
    public class PublicSectorOrganisationsController : Controller
    {
        private readonly IMediator _mediator;

        public PublicSectorOrganisationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediator.Send(new GetPublicSectorOrganisationByIdQuery { Id = id});

            if(response == null)
                return NotFound();

            return new OkObjectResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMatches([FromQuery] string? searchTerm)
        {
            var response = await _mediator.Send(new GetMatchingPublicSectorOrganisationsQuery
            {
                SearchTerm = searchTerm
            });

            return new OkObjectResult(response);
        }
    }
}
