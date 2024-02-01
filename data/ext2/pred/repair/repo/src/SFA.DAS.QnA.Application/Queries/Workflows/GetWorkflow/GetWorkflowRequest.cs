using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Workflows.GetWorkflow
{
    public class GetWorkflowRequest : IRequest<HandlerResponse<Workflow>>
    {
        public Guid ProjectId { get; }
        public Guid WorkflowId { get; }

        public GetWorkflowRequest(Guid projectId, Guid workflowId)
        {
            ProjectId = projectId;
            WorkflowId = workflowId;
        }
    }
}