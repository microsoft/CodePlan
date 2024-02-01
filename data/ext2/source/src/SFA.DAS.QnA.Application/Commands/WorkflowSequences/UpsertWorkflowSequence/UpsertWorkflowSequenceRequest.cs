using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSequences.UpsertWorkflowSequence
{
    public class UpsertWorkflowSequenceRequest : IRequest<HandlerResponse<WorkflowSequence>>
    {
        public Guid WorkflowId { get; }
        public Guid SequenceId { get; }
        public WorkflowSequence Sequence { get; }

        public UpsertWorkflowSequenceRequest(Guid workflowId, Guid sequenceId, WorkflowSequence sequence)
        {
            WorkflowId = workflowId;
            SequenceId = sequenceId;
            Sequence = sequence;
        }
    }
}