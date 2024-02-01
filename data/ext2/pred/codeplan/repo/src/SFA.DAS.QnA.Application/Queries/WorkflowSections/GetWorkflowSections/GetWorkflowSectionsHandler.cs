using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.WorkflowSections.GetWorkflowSections
{
    public class GetWorkflowSectionsHandler : IRequestHandler<GetWorkflowSectionsRequest, HandlerResponse<List<WorkflowSection>>>
    {
        private readonly QnaDataContext _dataContext;

        public GetWorkflowSectionsHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<List<WorkflowSection>>> Handle(GetWorkflowSectionsRequest request, CancellationToken cancellationToken)
        {
            var sections = await _dataContext.WorkflowSections.AsNoTracking().Where(sec => sec.ProjectId == request.ProjectId).ToListAsync(cancellationToken);
            return !sections.Any()
                ? new HandlerResponse<List<WorkflowSection>>(success: false, message: "No sections exist for this Project ID")
                : new HandlerResponse<List<WorkflowSection>>(sections);
        }
    }
}