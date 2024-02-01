using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback
{
    public class UpsertFeedbackHandler : IRequestHandler<UpsertFeedbackRequest, HandlerResponse<Page>>
    {
        private readonly QnaDataContext _dataContext;

        public UpsertFeedbackHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Page>> Handle(UpsertFeedbackRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.ApplicationId == request.ApplicationId && sec.Id == request.SectionId, cancellationToken);
            if (section is null) return new HandlerResponse<Page>(success: false, message: $"SectionId {request.SectionId} does not exist in ApplicationId {request.ApplicationId}");

            var qnaData = new QnAData(section.QnAData);

            var page = qnaData.Pages.SingleOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<Page>(success: false, message: $"PageId {request.PageId} does not exist");

            if (page.Feedback is null)
            {
                page.Feedback = new List<Feedback>();
            }

            if (page.Feedback.All(f => f.Id != request.Feedback.Id))
            {
                request.Feedback.Date = SystemTime.UtcNow();
                request.Feedback.IsNew = true;
                page.Feedback.Add(request.Feedback);
            }
            else
            {
                var existingFeedback = page.Feedback.Single(f => f.Id == request.Feedback.Id);
                existingFeedback.Date = request.Feedback.Date;
                existingFeedback.From = request.Feedback.From;
                existingFeedback.Message = request.Feedback.Message;
                existingFeedback.IsCompleted = request.Feedback.IsCompleted;
                existingFeedback.IsNew = request.Feedback.IsNew;
            }

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<Page>(page);
        }
    }
}