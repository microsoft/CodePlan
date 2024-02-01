using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.RemovePageAnswer
{
    public class RemovePageAnswerRequest : IRequest<HandlerResponse<RemovePageAnswerResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public Guid AnswerId { get; }

        public RemovePageAnswerRequest(Guid applicationId, Guid sectionId, string pageId, Guid answerId)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            AnswerId = answerId;
        }
    }
}