using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage
{
    public class CanUpdatePageRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }

        public CanUpdatePageRequest(Guid applicationId, Guid sectionId, string pageId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}