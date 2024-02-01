using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetPage
{
    public class GetPageRequest : IRequest<HandlerResponse<Page>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }

        public GetPageRequest(Guid applicationId, Guid sectionId, string pageId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}