using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.WorkflowSequences.CreateWorkflowSequence;
using SFA.DAS.QnA.Application.Commands.WorkflowSequences.UpsertWorkflowSequence;
using SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequence;
using SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequences;

namespace SFA.DAS.QnA.Api.Controllers.Config
{
    [Route("/config/workflows")]
    public class WorkflowSequencesController : Controller
    {
        private readonly IMediator _mediator;

        public WorkflowSequencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{workflowId}/sequences")]
        public async Task<ActionResult<List<WorkflowSequence>>> GetSequences(Guid workflowId)
        {
            var getWorkflowSequencesResult = await _mediator.Send(new GetWorkflowSequencesRequest(workflowId));
            if (!getWorkflowSequencesResult.Success) return NotFound(new NotFoundError(getWorkflowSequencesResult.Message));

            return getWorkflowSequencesResult.Value;
        }

        [HttpGet("{workflowId}/sequences/{sequenceId}")]
        public async Task<ActionResult<WorkflowSequence>> GetSequence(Guid workflowId, Guid sequenceId)
        {
            var getWorkflowSequenceResult = await _mediator.Send(new GetWorkflowSequenceRequest(workflowId, sequenceId));
            if (!getWorkflowSequenceResult.Success) return NotFound(new NotFoundError(getWorkflowSequenceResult.Message));

            return getWorkflowSequenceResult.Value;
        }

        [HttpPut("{workflowId}/sequences/{sequenceId}")]
        public async Task<ActionResult<WorkflowSequence>> UpsertWorkflowSequence(Guid workflowId, Guid sequenceId, [FromBody] WorkflowSequence sequence)
        {
            var upsertWorkflowSequenceResponse = await _mediator.Send(new UpsertWorkflowSequenceRequest(workflowId, sequenceId, sequence));
            if (!upsertWorkflowSequenceResponse.Success) return BadRequest(new BadRequestError(upsertWorkflowSequenceResponse.Message));

            return upsertWorkflowSequenceResponse.Value;
        }

        [HttpPost("{workflowId}/sequences")]
        public async Task<ActionResult<WorkflowSequence>> CreateWorkflowSequence(Guid workflowId, [FromBody] WorkflowSequence sequence)
        {
            var createWorkflowSequenceResponse = await _mediator.Send(new CreateWorkflowSequenceRequest(workflowId, sequence));
            if (!createWorkflowSequenceResponse.Success) return BadRequest(new BadRequestError(createWorkflowSequenceResponse.Message));

            return createWorkflowSequenceResponse.Value;
        }
    }
}