using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSequences.UpsertWorkflowSequence
{
    public class UpsertWorkflowSequenceHandler : IRequestHandler<UpsertWorkflowSequenceRequest, HandlerResponse<WorkflowSequence>>
    {
        private readonly QnaDataContext _dataContext;

        public UpsertWorkflowSequenceHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<WorkflowSequence>> Handle(UpsertWorkflowSequenceRequest request, CancellationToken cancellationToken)
        {
            var existingSequence = await _dataContext.WorkflowSequences.SingleOrDefaultAsync(sequence => sequence.Id == request.SequenceId && sequence.WorkflowId == request.WorkflowId, cancellationToken: cancellationToken);
            if (existingSequence == null)
            {
                await _dataContext.WorkflowSequences.AddAsync(request.Sequence, cancellationToken);
            }
            else
            {
                existingSequence.IsActive = request.Sequence.IsActive;
                existingSequence.SectionId = request.Sequence.SectionId;
                existingSequence.SectionNo = request.Sequence.SectionNo;
                existingSequence.SequenceNo = request.Sequence.SequenceNo;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<WorkflowSequence>(existingSequence);
        }
    }
}