using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Projects.UpsertProject
{
    public class UpsertProjectRequest : IRequest<HandlerResponse<Project>>
    {
        public Guid ProjectId { get; }
        public Project Project { get; }

        public UpsertProjectRequest(Guid projectId, Project project)
        {
            ProjectId = projectId;
            Project = project;
        }
    }
}