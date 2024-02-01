using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence
{
    public class GetCurrentSequenceRequest : IRequest<HandlerResponse<Sequence>>
    {
        public Guid ApplicationId { get; }

        public GetCurrentSequenceRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}