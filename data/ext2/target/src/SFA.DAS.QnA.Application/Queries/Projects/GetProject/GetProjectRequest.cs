using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Projects.GetProject
{
    public class GetProjectRequest : IRequest<HandlerResponse<Project>>
    {
        public Guid ProjectId { get; }

        public GetProjectRequest(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}