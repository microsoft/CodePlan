using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.CreateSnapshot;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/applications")]
    [Produces("application/json")]
    [ApiController]
    public class ApplicationSnapshotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationSnapshotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Creates a snapshot of the requested application.
        /// </summary>
        /// <remarks>
        ///     Currently used for RoATP. The intention is to have a simple facility which allows audits
        ///     across the history of an application being submitted for each review cycle.
        /// </remarks>
        /// <param name="applicationId">The Id of the application which is to be snapshot</param>
        /// <returns>The newly created application's Id</returns>
        /// <response code="201">Returns the newly created application's Id</response>
        /// <response code="404">If the requested application could not be found</response>
        [HttpPost("{applicationId}/snapshot")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CreateSnapshotResponse>> CreateSnapshot(Guid applicationId)
        {
            var newSnapshot = await _mediator.Send(new CreateSnapshotRequest(applicationId));

            if (!newSnapshot.Success) return NotFound(new NotFoundError(newSnapshot.Message));

            return newSnapshot.Value;
        }
    }
}

