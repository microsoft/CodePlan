using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequence
{
    public class GetSequenceBySequenceNoRequest : IRequest<HandlerResponse<Sequence>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }

        public GetSequenceBySequenceNoRequest(Guid applicationId, int sequenceNo)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
        }
    }
}