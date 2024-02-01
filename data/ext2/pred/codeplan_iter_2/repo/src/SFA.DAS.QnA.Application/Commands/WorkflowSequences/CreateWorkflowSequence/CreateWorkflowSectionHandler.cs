using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSequences.CreateWorkflowSequence
{
    public class CreateWorkflowSequenceHandler : IRequestHandler<CreateWorkflowSequenceRequest, HandlerResponse<WorkflowSequence>>
    {
        private readonly QnaDataContext _dataContext;

        public CreateWorkflowSequenceHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<HandlerResponse<WorkflowSequence>> Handle(CreateWorkflowSequenceRequest request, CancellationToken cancellationToken)
        {
            await _dataContext.WorkflowSequences.AddAsync(request.Sequence, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return new HandlerResponse<WorkflowSequence>(request.Sequence);
        }
    }
}