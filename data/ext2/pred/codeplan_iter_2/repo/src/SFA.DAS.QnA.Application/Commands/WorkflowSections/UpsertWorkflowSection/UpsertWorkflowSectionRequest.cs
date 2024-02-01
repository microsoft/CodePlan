using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSections.UpsertWorkflowSection
{
    public class UpsertWorkflowSectionRequest : IRequest<HandlerResponse<WorkflowSection>>
    {
        public Guid ProjectId { get; }
        public Guid SectionId { get; }
        public WorkflowSection Section { get; }

        public UpsertWorkflowSectionRequest(Guid projectId, Guid sectionId, WorkflowSection section)
        {
            ProjectId = projectId;
            SectionId = sectionId;
            Section = section;
        }
    }
}