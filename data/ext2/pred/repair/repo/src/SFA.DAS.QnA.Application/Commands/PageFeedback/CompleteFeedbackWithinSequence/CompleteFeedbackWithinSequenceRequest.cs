using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.PageFeedback.CompleteFeedbackWithinSequence
{
    public class CompleteFeedbackWithinSequenceRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public Guid SequenceId { get; }

        public CompleteFeedbackWithinSequenceRequest(Guid applicationId, Guid sequenceId)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
        }
    }
}