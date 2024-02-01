using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSections.CreateWorkflowSection
{
    public class CreateWorkflowSectionHandler : IRequestHandler<CreateWorkflowSectionRequest, HandlerResponse<WorkflowSection>>
    {
        private readonly QnaDataContext _dataContext;

        public CreateWorkflowSectionHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<HandlerResponse<WorkflowSection>> Handle(CreateWorkflowSectionRequest request, CancellationToken cancellationToken)
        {
            await _dataContext.WorkflowSections.AddAsync(request.Section, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return new HandlerResponse<WorkflowSection>(request.Section);
        }
    }
}