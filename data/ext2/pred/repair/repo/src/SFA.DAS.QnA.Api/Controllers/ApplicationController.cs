using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/applications")]
    [Produces("application/json")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IMediator _mediator;

        public ApplicationController(ILogger<ApplicationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     Creates and starts a new application
        /// </summary>
        /// <param name="request">The required parameters to start the application</param>
        /// <returns>The newly created application's Id</returns>
        /// <response code="201">Returns the newly created application's Id</response>
        /// <response code="400">If the WorkflowType does not exist or the ApplicationData supplied does not match the Project's schema.</response>
        [HttpPost("start")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> StartApplication([FromBody] StartApplicationRequest request)
        {
            var newApplicationResponse = await _mediator.Send(request);

            if (!newApplicationResponse.Success)
            {
                _logger.LogError($"Unable to start application | Reason : {newApplicationResponse.Message}");
                return BadRequest(new BadRequestError(newApplicationResponse.Message));
            }

            return Ok(new { newApplicationResponse.Value.ApplicationId });
        }
    }
}