using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequences
{
    public class GetSequencesRequest : IRequest<HandlerResponse<List<Sequence>>>
    {
        public Guid ApplicationId { get; }

        public GetSequencesRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}