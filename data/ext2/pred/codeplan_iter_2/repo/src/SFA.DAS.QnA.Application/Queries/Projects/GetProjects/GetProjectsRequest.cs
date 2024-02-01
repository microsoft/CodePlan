using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Projects.GetProjects
{
    public class GetProjectsRequest : IRequest<HandlerResponse<List<Project>>>
    {

    }
}