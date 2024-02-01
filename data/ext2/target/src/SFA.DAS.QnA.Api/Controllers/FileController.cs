using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.DeleteFile;
using SFA.DAS.QnA.Application.Commands.Files.DownloadFile;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IMediator _mediator;

        public FileController(ILogger<FileController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/upload")]
        public async Task<ActionResult<SetPageAnswersResponse>> Upload(Guid applicationId, Guid sectionId, string pageId)
        {
            IFormFileCollection files;
            try
            {
                files = HttpContext.Request.Form.Files;
            }
            catch
            {
                files = null;
            }

            var uploadResult = await _mediator.Send(new SubmitPageOfFilesRequest(applicationId, sectionId, pageId, files));

            if (!uploadResult.Success)
            {
                _logger.LogError($"Unable to upload file for page {pageId} | Reason : {uploadResult.Message}");
                return BadRequest(new BadRequestError(uploadResult.Message));
            }

            return uploadResult.Value;
        }

        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/download")]
        public async Task<IActionResult> DownloadPageZipOfFiles(Guid applicationId, Guid sectionId, string pageId)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId, null, null));

            if (!downloadResult.Success)
            {
                _logger.LogError($"Unable to download files for page {pageId} | Reason : {downloadResult.Message}");
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;

            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }

        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download")]
        public async Task<IActionResult> DownloadFileOrZipOfFiles(Guid applicationId, Guid sectionId, string pageId, string questionId)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId, questionId, null));

            if (!downloadResult.Success)
            {
                _logger.LogError($"Unable to download files for question {questionId} | Reason : {downloadResult.Message}");
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;

            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }

        [HttpGet("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}/questions/{questionId}/download")]
        public async Task<IActionResult> DownloadFileOrZipOfFiles(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId)
        {
            var downloadResult = await _mediator.Send(new DownloadFileBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId, questionId, null));

            if (!downloadResult.Success)
            {
                _logger.LogError($"Unable to download files for question {questionId} | Reason : {downloadResult.Message}");
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;

            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }

        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            var downloadResult = await _mediator.Send(new DownloadFileRequest(applicationId, sectionId, pageId, questionId, fileName));

            if (!downloadResult.Success)
            {
                _logger.LogError($"Unable to download file for question {questionId} | Reason : {downloadResult.Message}");
                return BadRequest(new BadRequestError(downloadResult.Message));
            }

            var downloadResultValue = downloadResult.Value;

            return File(downloadResultValue.Stream, downloadResultValue.ContentType, downloadResultValue.FileName);
        }

        [HttpDelete("{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        public async Task<IActionResult> DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            var deleteFileResponse = await _mediator.Send(new DeleteFileRequest(applicationId, sectionId, pageId, questionId, fileName));

            if (!deleteFileResponse.Success)
            {
                _logger.LogError($"Unable to delete file for question {questionId} | Reason : {deleteFileResponse.Message}");
                return BadRequest(new BadRequestError(deleteFileResponse.Message));
            }

            return Ok();
        }
    }
}