using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersBySectionNoRequest : IRequest<HandlerResponse<SetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }
        public List<Answer> Answers { get; }

        public SetPageAnswersBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId, List<Answer> answers)
        {
            ApplicationId = applicationId;
            PageId = pageId;
            Answers = answers;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
        }
    }
}