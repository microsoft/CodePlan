using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    public class DownloadFileBySectionNoHandler : IRequestHandler<DownloadFileBySectionNoRequest, HandlerResponse<DownloadFile>>
    {
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;
        private readonly QnaDataContext _dataContext;

        public DownloadFileBySectionNoHandler(IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, QnaDataContext dataContext)
        {
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<DownloadFile>> Handle(DownloadFileBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.SectionNo == request.SectionNo && sec.SequenceNo == request.SequenceNo && sec.ApplicationId == request.ApplicationId, cancellationToken);

            if (section == null)
            {
                return new HandlerResponse<DownloadFile>(success: false, message: $"Section {request.SectionNo} in Application {request.ApplicationId} does not exist.");
            }

            var downloadSerivce = new DownloadFileService(_fileStorageConfig, _encryptionService);
            return await downloadSerivce.GetDownloadFile(request.ApplicationId, section, request.PageId, request.QuestionId, request.FileName, cancellationToken);
        }
    }
}