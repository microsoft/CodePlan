using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.PageFeedback.DeleteFeedback
{
    public class DeleteFeedbackHandler : IRequestHandler<DeleteFeedbackRequest, HandlerResponse<Page>>
    {
        private readonly QnaDataContext _dataContext;

        public DeleteFeedbackHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Page>> Handle(DeleteFeedbackRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.ApplicationId == request.ApplicationId && sec.Id == request.SectionId, cancellationToken);
            if (section is null) return new HandlerResponse<Page>(success: false, message: $"SectionId {request.SectionId} does not exist in ApplicationId {request.ApplicationId}");

            var qnaData = new QnAData(section.QnAData);

            var page = qnaData.Pages.SingleOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<Page>(success: false, message: $"PageId {request.PageId} does not exist");

            if (page.Feedback is null) return new HandlerResponse<Page>(success: false, message: $"Feedback {request.FeedbackId} does not exist");

            var existingFeedback = page.Feedback.SingleOrDefault(f => f.Id == request.FeedbackId);

            if (existingFeedback is null) return new HandlerResponse<Page>(success: false, message: $"Feedback {request.FeedbackId} does not exist");

            page.Feedback.Remove(existingFeedback);

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<Page>(page);
        }
    }
}