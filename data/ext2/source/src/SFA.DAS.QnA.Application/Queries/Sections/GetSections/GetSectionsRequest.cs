using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSections
{
    public class GetSectionsRequest : IRequest<HandlerResponse<List<Section>>>
    {
        public Guid ApplicationId { get; }

        public GetSectionsRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
