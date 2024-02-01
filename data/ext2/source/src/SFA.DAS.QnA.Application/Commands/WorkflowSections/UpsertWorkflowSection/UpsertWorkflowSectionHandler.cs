using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.WorkflowSections.UpsertWorkflowSection
{
    public class UpsertWorkflowSectionHandler : IRequestHandler<UpsertWorkflowSectionRequest, HandlerResponse<WorkflowSection>>
    {
        private readonly QnaDataContext _dataContext;

        public UpsertWorkflowSectionHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<WorkflowSection>> Handle(UpsertWorkflowSectionRequest request, CancellationToken cancellationToken)
        {
            var existingSection = await _dataContext.WorkflowSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ProjectId == request.ProjectId, cancellationToken: cancellationToken);
            if (existingSection == null)
            {
                await _dataContext.WorkflowSections.AddAsync(request.Section, cancellationToken);
            }
            else
            {
                existingSection.Title = request.Section.Title;
                existingSection.DisplayType = request.Section.DisplayType;
                existingSection.LinkTitle = request.Section.LinkTitle;
                existingSection.QnAData = request.Section.QnAData;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<WorkflowSection>(existingSection);
        }
    }
}