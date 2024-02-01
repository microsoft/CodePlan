using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class GetWorkflowsRequest : IRequest<HandlerResponse<List<Workflow>>>
    {
        public Guid ProjectId { get; }

        public GetWorkflowsRequest(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}