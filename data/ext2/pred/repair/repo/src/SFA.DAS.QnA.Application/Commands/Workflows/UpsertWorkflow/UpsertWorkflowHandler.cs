using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Workflows.UpsertWorkflow
{
    public class UpsertWorkflowHandler : IRequestHandler<UpsertWorkflowRequest, HandlerResponse<Workflow>>
    {
        private readonly QnaDataContext _dataContext;

        public UpsertWorkflowHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Workflow>> Handle(UpsertWorkflowRequest request, CancellationToken cancellationToken)
        {
            var existingWorkflow = await _dataContext.Workflows.SingleOrDefaultAsync(sec => sec.Id == request.WorkflowId && sec.ProjectId == request.ProjectId, cancellationToken: cancellationToken);
            if (existingWorkflow == null)
            {
                request.Workflow.ProjectId = request.ProjectId;
                await _dataContext.Workflows.AddAsync(request.Workflow, cancellationToken);
            }
            else
            {
                existingWorkflow.Status = request.Workflow.Status;
                existingWorkflow.Description = request.Workflow.Description;
                existingWorkflow.Type = request.Workflow.Type;
                existingWorkflow.Version = request.Workflow.Version;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<Workflow>(existingWorkflow);
        }
    }
}