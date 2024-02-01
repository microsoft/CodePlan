using MediatR;
using SFA.DAS.QnA.Api.Types;
using System;

namespace SFA.DAS.QnA.Application.Commands.SkipPage
{
    public class SkipPageRequest : IRequest<HandlerResponse<SkipPageResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }

        public SkipPageRequest(Guid applicationId, Guid sectionId, string pageId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}
