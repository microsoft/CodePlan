using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSection
{
    public class GetWorkflowSectionRequest : IRequest<HandlerResponse<WorkflowSection>>
    {
        public Guid ProjectId { get; }
        public Guid SectionId { get; }


        public GetWorkflowSectionRequest(Guid projectId, Guid sectionId)
        {
            ProjectId = projectId;
            SectionId = sectionId;
        }
    }
}