using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequence
{
    public class GetSequenceRequest : IRequest<HandlerResponse<Sequence>>
    {
        public Guid ApplicationId { get; }
        public Guid SequenceId { get; }

        public GetSequenceRequest(Guid applicationId, Guid sequenceId)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
        }
    }
}