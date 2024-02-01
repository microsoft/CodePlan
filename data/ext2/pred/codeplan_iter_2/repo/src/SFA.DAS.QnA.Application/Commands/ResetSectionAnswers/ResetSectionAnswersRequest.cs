using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetSectionAnswersRequest : IRequest<HandlerResponse<ResetSectionAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }

        public ResetSectionAnswersRequest(Guid applicationId, int sequenceNo, int sectionNo)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
        }
    }
}