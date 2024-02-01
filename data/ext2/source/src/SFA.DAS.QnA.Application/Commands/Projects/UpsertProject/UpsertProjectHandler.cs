using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Projects.UpsertProject
{
    public class UpsertProjectHandler : IRequestHandler<UpsertProjectRequest, HandlerResponse<Project>>
    {
        private readonly QnaDataContext _dataContext;

        public UpsertProjectHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Project>> Handle(UpsertProjectRequest request, CancellationToken cancellationToken)
        {
            var existingProject = await _dataContext.Projects.SingleOrDefaultAsync(project => project.Id == request.ProjectId, cancellationToken: cancellationToken);
            if (existingProject == null)
            {
                await _dataContext.Projects.AddAsync(request.Project, cancellationToken);
            }
            else
            {
                existingProject.Name = request.Project.Name;
                existingProject.ApplicationDataSchema = request.Project.ApplicationDataSchema;
                existingProject.CreatedBy = request.Project.CreatedBy;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<Project>(existingProject);
        }
    }
}