using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetPageAnswersRequest : IRequest<HandlerResponse<ResetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }

        public ResetPageAnswersRequest(Guid applicationId, Guid sectionId, string pageId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}
