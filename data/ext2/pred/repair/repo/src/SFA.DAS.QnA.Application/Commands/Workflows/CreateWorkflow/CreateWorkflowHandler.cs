using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Workflows.CreateWorkflow
{
    public class CreateWorkflowHandler : IRequestHandler<CreateWorkflowRequest, HandlerResponse<Workflow>>
    {
        private readonly QnaDataContext _dataContext;

        public CreateWorkflowHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<HandlerResponse<Workflow>> Handle(CreateWorkflowRequest request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.SingleOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken: cancellationToken);

            request.Workflow.ApplicationDataSchema = project.ApplicationDataSchema;

            await _dataContext.Workflows.AddAsync(request.Workflow, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return new HandlerResponse<Workflow>(request.Workflow);
        }
    }
}