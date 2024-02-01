using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequence
{
    public class GetWorkflowSequenceHandler : IRequestHandler<GetWorkflowSequenceRequest, HandlerResponse<WorkflowSequence>>
    {
        private readonly QnaDataContext _dataContext;

        public GetWorkflowSequenceHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<WorkflowSequence>> Handle(GetWorkflowSequenceRequest request, CancellationToken cancellationToken)
        {
            var sequence = await _dataContext.WorkflowSequences.AsNoTracking().SingleOrDefaultAsync(seq => seq.Id == request.SequenceId && seq.WorkflowId == request.WorkflowId, cancellationToken);
            return sequence is null
                ? new HandlerResponse<WorkflowSequence>(success: false, message: "Project or WorkflowSection does not exist")
                : new HandlerResponse<WorkflowSequence>(sequence);
        }
    }
}