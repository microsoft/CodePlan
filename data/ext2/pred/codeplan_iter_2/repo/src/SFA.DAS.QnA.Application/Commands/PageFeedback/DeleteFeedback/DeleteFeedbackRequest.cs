using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.PageFeedback.DeleteFeedback
{
    public class DeleteFeedbackRequest : IRequest<HandlerResponse<Page>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public Guid FeedbackId { get; }

        public DeleteFeedbackRequest(Guid applicationId, Guid sectionId, string pageId, Guid feedbackId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            FeedbackId = feedbackId;
        }
    }
}