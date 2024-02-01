using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSequences.GetWorkflowSequences
{
    public class GetWorkflowSequencesHandler : IRequestHandler<GetWorkflowSequencesRequest, HandlerResponse<List<WorkflowSequence>>>
    {
        private readonly QnaDataContext _dataContext;

        public GetWorkflowSequencesHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<List<WorkflowSequence>>> Handle(GetWorkflowSequencesRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _dataContext.WorkflowSequences.AsNoTracking().Where(seq => seq.WorkflowId == request.WorkflowId).ToListAsync(cancellationToken);

            return !sequences.Any()
                ? new HandlerResponse<List<WorkflowSequence>>(success: false, message: "No sequences exist for this Workflow ID")
                : new HandlerResponse<List<WorkflowSequence>>(sequences);
        }
    }
}