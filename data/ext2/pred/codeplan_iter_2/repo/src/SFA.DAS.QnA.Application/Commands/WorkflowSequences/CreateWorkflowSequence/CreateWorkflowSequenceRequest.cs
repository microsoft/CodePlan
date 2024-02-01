using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSequences.CreateWorkflowSequence
{
    public class CreateWorkflowSequenceRequest : IRequest<HandlerResponse<WorkflowSequence>>
    {
        public Guid WorkflowId { get; }
        public WorkflowSequence Sequence { get; }

        public CreateWorkflowSequenceRequest(Guid workflowId, WorkflowSequence sequence)
        {
            WorkflowId = workflowId;
            Sequence = sequence;
        }
    }
}