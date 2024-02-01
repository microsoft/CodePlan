using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetPageAnswersBySectionNoRequest : IRequest<HandlerResponse<ResetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }

        public ResetPageAnswersBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
        }
    }
}