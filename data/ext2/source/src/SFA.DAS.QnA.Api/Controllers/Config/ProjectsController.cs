using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Projects.CreateProject;
using SFA.DAS.QnA.Application.Commands.Projects.UpsertProject;
using SFA.DAS.QnA.Application.Queries.Projects.GetProject;
using SFA.DAS.QnA.Application.Queries.Projects.GetProjects;

namespace SFA.DAS.QnA.Api.Controllers.Config
{
    [Route("/config/projects")]
    public class ProjectsController : Controller
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Project>>> GetProjects()
        {
            var getProjectsResult = await _mediator.Send(new GetProjectsRequest());
            if (!getProjectsResult.Success) return NotFound(new NotFoundError(getProjectsResult.Message));

            return getProjectsResult.Value;
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<Project>> GetProject(Guid projectId)
        {
            var getProjectResult = await _mediator.Send(new GetProjectRequest(projectId));
            if (!getProjectResult.Success) return NotFound(new NotFoundError(getProjectResult.Message));

            return getProjectResult.Value;
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult<Project>> UpsertProject(Guid projectId, [FromBody] Project project)
        {
            var upsertProjectResult = await _mediator.Send(new UpsertProjectRequest(projectId, project));
            if (!upsertProjectResult.Success) return BadRequest(new BadRequestError(upsertProjectResult.Message));

            return upsertProjectResult.Value;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
        {
            var createProjectResult = await _mediator.Send(new CreateProjectRequest(project));
            if (!createProjectResult.Success) return BadRequest(new BadRequestError(createProjectResult.Message));

            return createProjectResult.Value;
        }
    }
}