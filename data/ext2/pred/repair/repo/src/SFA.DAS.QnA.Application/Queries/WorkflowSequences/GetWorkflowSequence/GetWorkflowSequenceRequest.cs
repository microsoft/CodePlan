using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequence
{
    public class GetWorkflowSequenceRequest : IRequest<HandlerResponse<WorkflowSequence>>
    {
        public Guid WorkflowId { get; }
        public Guid SequenceId { get; }

        public GetWorkflowSequenceRequest(Guid workflowId, Guid sequenceId)
        {
            WorkflowId = workflowId;
            SequenceId = sequenceId;
        }
    }
}