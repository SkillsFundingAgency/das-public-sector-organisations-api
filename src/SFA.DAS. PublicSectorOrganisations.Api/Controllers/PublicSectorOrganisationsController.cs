using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetAllPublicSectorOrganisations;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetPublicSectorOrganisationById;

namespace SFA.DAS.PublicSectorOrganisations.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
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
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllPublicSectorOrganisationsQuery());

            return new OkObjectResult(response);
        }


    }
}
