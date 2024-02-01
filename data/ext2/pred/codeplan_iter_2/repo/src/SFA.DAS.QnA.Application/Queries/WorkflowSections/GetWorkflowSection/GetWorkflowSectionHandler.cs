using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSection
{
    public class GetWorkflowSectionHandler : IRequestHandler<GetWorkflowSectionRequest, HandlerResponse<WorkflowSection>>
    {
        private readonly QnaDataContext _dataContext;

        public GetWorkflowSectionHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<WorkflowSection>> Handle(GetWorkflowSectionRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.WorkflowSections.AsNoTracking().SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ProjectId == request.ProjectId, cancellationToken);
            return section is null
                ? new HandlerResponse<WorkflowSection>(success: false, message: "Project or WorkflowSection does not exist")
                : new HandlerResponse<WorkflowSection>(section);
        }
    }
}