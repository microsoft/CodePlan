using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    public class DownloadFileRequest : IRequest<HandlerResponse<DownloadFile>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }
        public string FileName { get; }

        public DownloadFileRequest(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
            FileName = fileName;
        }
    }
}