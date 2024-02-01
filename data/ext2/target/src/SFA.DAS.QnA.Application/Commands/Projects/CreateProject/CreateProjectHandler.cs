using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Projects.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectRequest, HandlerResponse<Project>>
    {
        private readonly QnaDataContext _dataContext;

        public CreateProjectHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<HandlerResponse<Project>> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
        {
            await _dataContext.Projects.AddAsync(request.Project, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return new HandlerResponse<Project>(request.Project);
        }
    }
}