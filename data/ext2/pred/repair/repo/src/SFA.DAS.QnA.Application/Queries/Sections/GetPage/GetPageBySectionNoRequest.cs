using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetPage
{
    public class GetPageBySectionNoRequest : IRequest<HandlerResponse<Page>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }

        public GetPageBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
        }
    }
}