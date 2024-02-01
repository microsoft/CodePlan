using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Projects.GetProjects
{
    public class GetProjectsHandler : IRequestHandler<GetProjectsRequest, HandlerResponse<List<Project>>>
    {
        private readonly QnaDataContext _dataContext;

        public GetProjectsHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<List<Project>>> Handle(GetProjectsRequest request, CancellationToken cancellationToken)
        {
            var projects = await _dataContext.Projects.AsNoTracking().ToListAsync(cancellationToken);

            return !projects.Any()
                ? new HandlerResponse<List<Project>>(success: false, message: "No Projects exist")
                : new HandlerResponse<List<Project>>(projects);
        }
    }
}