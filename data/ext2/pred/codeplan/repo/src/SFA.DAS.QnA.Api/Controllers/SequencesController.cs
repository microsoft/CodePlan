using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequence;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class SequencesController : Controller
    {
        private readonly IMediator _mediator;

        public SequencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns all of the Sequences for the Application
        /// </summary>
        /// <returns>An array of Sequences</returns>
        /// <response code="200">Returns the Application's Sequences</response>
        /// <response code="204">If there are no Sequences for the given Application Id</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [HttpGet("{applicationId}/sequences")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<Sequence>>> GetSequences(Guid applicationId)
        {
            var sequences = await _mediator.Send(new GetSequencesRequest(applicationId), CancellationToken.None);
            if (!sequences.Success) return NotFound(new NotFoundError(sequences.Message));
            if (sequences.Value.Count == 0) return NoContent();

            return sequences.Value;
        }

        /// <summary>
        ///     Returns the requested Sequence
        /// </summary>
        /// <returns>The requested Sequence</returns>
        /// <response code="200">Returns the requested sequence</response>
        /// <response code="404">If the ApplicationId or SequenceId are not found</response>
        [HttpGet("{applicationId}/sequences/{sequenceId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Sequence>> GetSequence(Guid applicationId, Guid sequenceId)
        {
            var sequence = await _mediator.Send(new GetSequenceRequest(applicationId, sequenceId), CancellationToken.None);
            if (!sequence.Success) return NotFound(new NotFoundError(sequence.Message));

            return sequence.Value;
        }

        /// <summary>
        ///     Returns the requested Sequence
        /// </summary>
        /// <returns>The requested Sequence</returns>
        /// <response code="200">Returns the requested sequence</response>
        /// <response code="404">If the ApplicationId or SequenceNo are not found</response>
        [HttpGet("{applicationId}/sequences/{sequenceNo:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Sequence>> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo)
        {
            var sequence = await _mediator.Send(new GetSequenceBySequenceNoRequest(applicationId, sequenceNo), CancellationToken.None);
            if (!sequence.Success) return NotFound(new NotFoundError(sequence.Message));

            return sequence.Value;
        }
    }
}