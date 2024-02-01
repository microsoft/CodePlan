using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSections
{
    public class GetWorkflowSectionsRequest : IRequest<HandlerResponse<List<WorkflowSection>>>
    {
        public Guid ProjectId { get; }

        public GetWorkflowSectionsRequest(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}