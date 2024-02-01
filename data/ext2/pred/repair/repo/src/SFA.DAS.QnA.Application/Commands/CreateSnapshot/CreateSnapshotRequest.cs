using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.CreateSnapshot
{
    public class CreateSnapshotRequest : IRequest<HandlerResponse<CreateSnapshotResponse>>
    {
        public Guid ApplicationId { get; }

        public CreateSnapshotRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
