using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PublicSectorOrganisations.Domain.Application.Commands.Import;

namespace SFA.DAS.PublicSectorOrganisations.Api.Controllers
{
    [ApiController]
    [Route("/dataload")]
    public class DataLoadController : Controller
    {
        private readonly IMediator _mediator;

        public DataLoadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> PostStart()
        {
            await _mediator.Send(new ImportCommand());

            return Ok();
        }
    }
}
