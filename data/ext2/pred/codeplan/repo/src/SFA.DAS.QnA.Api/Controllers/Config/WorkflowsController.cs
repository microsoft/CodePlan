using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Workflows.CreateWorkflow;
using SFA.DAS.QnA.Application.Commands.Workflows.UpsertWorkflow;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.QnA.Application.Queries.Workflows.GetWorkflow;

namespace SFA.DAS.QnA.Api.Controllers.Config
{
    [Route("/config/projects")]
    public class WorkflowsController : Controller
    {
        private readonly IMediator _mediator;

        public WorkflowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{projectId}/workflows")]
        public async Task<ActionResult<List<Workflow>>> GetWorkflows(Guid projectId)
        {
            var getWorkflowResponse = await _mediator.Send(new GetWorkflowsRequest(projectId));
            if (!getWorkflowResponse.Success) return NotFound(new NotFoundError(getWorkflowResponse.Message));

            return getWorkflowResponse.Value;
        }

        [HttpGet("{projectId}/workflows/{workflowId}")]
        public async Task<ActionResult<Workflow>> GetWorkflow(Guid projectId, Guid workflowId)
        {
            var getWorkflowResponse = await _mediator.Send(new GetWorkflowRequest(projectId, workflowId));
            if (!getWorkflowResponse.Success) return NotFound(new NotFoundError(getWorkflowResponse.Message));

            return getWorkflowResponse.Value;
        }

        [HttpPut("{projectId}/workflows/{workflowId}")]
        public async Task<ActionResult<Workflow>> UpsertWorkflow(Guid projectId, Guid workflowId, [FromBody] Workflow workflow)
        {
            var upsertWorkflowResponse = await _mediator.Send(new UpsertWorkflowRequest(projectId, workflowId, workflow));
            if (!upsertWorkflowResponse.Success) return BadRequest(new BadRequestError(upsertWorkflowResponse.Message));

            return upsertWorkflowResponse.Value;
        }

        [HttpPost("{projectId}/workflows")]
        public async Task<ActionResult<Workflow>> CreateWorkflow(Guid projectId, [FromBody] Workflow workflow)
        {
            var createWorkflowResponse = await _mediator.Send(new CreateWorkflowRequest(projectId, workflow));
            if (!createWorkflowResponse.Success) return BadRequest(new BadRequestError(createWorkflowResponse.Message));

            return createWorkflowResponse.Value;
        }
    }
}