using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersRequest : IRequest<HandlerResponse<SetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public List<Answer> Answers { get; }

        public SetPageAnswersRequest(Guid applicationId, Guid sectionId, string pageId, List<Answer> answers)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            Answers = answers;
        }
    }
}