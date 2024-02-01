using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Workflows.UpsertWorkflow
{
    public class UpsertWorkflowRequest : IRequest<HandlerResponse<Workflow>>
    {
        public Guid ProjectId { get; }
        public Guid WorkflowId { get; }
        public Workflow Workflow { get; }

        public UpsertWorkflowRequest(Guid projectId, Guid workflowId, Workflow workflow)
        {
            ProjectId = projectId;
            WorkflowId = workflowId;
            Workflow = workflow;
        }
    }
}