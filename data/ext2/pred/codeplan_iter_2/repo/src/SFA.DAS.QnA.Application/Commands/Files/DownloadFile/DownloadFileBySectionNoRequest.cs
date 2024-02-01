using System;
using MediatR;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    public class DownloadFileBySectionNoRequest : IRequest<HandlerResponse<DownloadFile>>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }
        public string QuestionId { get; }
        public string FileName { get; }

        public DownloadFileBySectionNoRequest(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId, string fileName)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
            QuestionId = questionId;
            FileName = fileName;
        }
    }
}