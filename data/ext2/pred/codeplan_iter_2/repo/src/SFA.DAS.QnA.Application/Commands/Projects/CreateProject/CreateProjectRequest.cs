using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Projects.CreateProject
{
    public class CreateProjectRequest : IRequest<HandlerResponse<Project>>
    {
        public Project Project { get; }

        public CreateProjectRequest(Project project)
        {
            Project = project;
        }
    }
}