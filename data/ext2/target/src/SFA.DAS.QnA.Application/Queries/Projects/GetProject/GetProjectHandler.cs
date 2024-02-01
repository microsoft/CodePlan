using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Projects.GetProject
{
    public class GetProjectHandler : IRequestHandler<GetProjectRequest, HandlerResponse<Project>>
    {
        private readonly QnaDataContext _dataContext;

        public GetProjectHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Project>> Handle(GetProjectRequest request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.AsNoTracking().SingleOrDefaultAsync(proj => proj.Id == request.ProjectId, cancellationToken);
            return project is null
                ? new HandlerResponse<Project>(success: false, message: "Project does not exist")
                : new HandlerResponse<Project>(project);
        }
    }
}