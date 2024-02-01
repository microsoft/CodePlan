using MediatR;
using SFA.DAS.QnA.Api.Types;
using System;

namespace SFA.DAS.QnA.Application.Commands.SkipPage
{
    public class SkipPageBySectionNoRequest : IRequest<HandlerResponse<SkipPageResponse>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }

        public SkipPageBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
        }
    }
}
