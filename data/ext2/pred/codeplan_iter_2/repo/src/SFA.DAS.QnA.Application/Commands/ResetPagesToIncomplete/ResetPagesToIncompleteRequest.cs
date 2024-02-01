using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.ResetPagesToIncomplete
{
    public class ResetPagesToIncompleteRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public List<string> PageIdsToExclude { get; }

        public ResetPagesToIncompleteRequest(Guid applicationId, int sequenceNo, int sectionNo, List<string> pageIdsToExclude)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageIdsToExclude = pageIdsToExclude;
        }
    }
}