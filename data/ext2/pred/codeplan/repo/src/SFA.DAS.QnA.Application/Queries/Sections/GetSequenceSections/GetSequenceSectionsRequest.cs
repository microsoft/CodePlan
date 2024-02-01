using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSequenceSections
{
    public class GetSequenceSectionsRequest : IRequest<HandlerResponse<List<Section>>>
    {
        public GetSequenceSectionsRequest(Guid applicationId, Guid sequenceId)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
        }

        public Guid ApplicationId { get; set; }
        public Guid SequenceId { get; set; }
    }
}