using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile
{
    public class SubmitPageOfFilesRequest : IRequest<HandlerResponse<SetPageAnswersResponse>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public IFormFileCollection Files { get; }

        public SubmitPageOfFilesRequest(Guid applicationId, Guid sectionId, string pageId, IFormFileCollection files)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            Files = files;
        }
    }
}