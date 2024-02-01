using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSections.CreateWorkflowSection
{
    public class CreateWorkflowSectionRequest : IRequest<HandlerResponse<WorkflowSection>>
    {
        public Guid ProjectId { get; }
        public WorkflowSection Section { get; }

        public CreateWorkflowSectionRequest(Guid projectId, WorkflowSection section)
        {
            ProjectId = projectId;
            Section = section;
        }
    }
}