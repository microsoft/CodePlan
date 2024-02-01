using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Workflows.CreateWorkflow
{
    public class CreateWorkflowRequest : IRequest<HandlerResponse<Workflow>>
    {
        public Guid ProjectId { get; }
        public Workflow Workflow { get; }

        public CreateWorkflowRequest(Guid projectId, Workflow workflow)
        {
            ProjectId = projectId;
            Workflow = workflow;
        }
    }
}