using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage
{
    public class CanUpdatePageBySectionNoRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }

        public CanUpdatePageBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
        }
    }
}