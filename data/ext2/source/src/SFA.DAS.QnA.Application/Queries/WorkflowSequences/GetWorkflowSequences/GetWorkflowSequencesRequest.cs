using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequences
{
    public class GetWorkflowSequencesRequest : IRequest<HandlerResponse<List<WorkflowSequence>>>
    {
        public Guid WorkflowId { get; }

        public GetWorkflowSequencesRequest(Guid workflowId)
        {
            WorkflowId = workflowId;
        }
    }
}