using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSection
{
    public class GetSectionBySectionNoRequest : IRequest<HandlerResponse<Section>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }

        public GetSectionBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
        }
    }
}