using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Application.Commands.Files;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StorageMigrationController : Controller
    {
        private QnaDataContext _dataContext;
        private IOptions<FileStorageConfig> _fileStorageConfig;
        private IEncryptionService _encryptionService;
        private readonly ILogger<StorageMigrationController> _log;

        public StorageMigrationController(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, ILogger<StorageMigrationController> log)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _log = log;
        }

        [HttpPost("/storageMigration")]
        public async Task<ActionResult<FileMigrationResult>> Migrate()
        {
            var result = new FileMigrationResult { MigratedFiles = new List<MigratedFile>() };

            try
            {
                // get all sections where SectionNo = 3
                var sections = await _dataContext.ApplicationSections.Where(sec => (sec.SectionNo == 3 && sec.SequenceNo == 1) || sec.SequenceNo == 2).ToListAsync();

                foreach (var section in sections)
                {
                    var sectionId = section.Id;


                    foreach (var page in section.QnAData.Pages)
                    {
                        if (page.Questions.Any(q => q.Input.Type == "FileUpload"))
                        {
                            var sequenceId = page.SequenceId.Value;
                            foreach (var pageOfAnswer in page.PageOfAnswers)
                            {
                                foreach (var answer in pageOfAnswer.Answers)
                                {
                                    if (!string.IsNullOrWhiteSpace(answer.Value))
                                    {
                                        // get original file...
                                        var account = CloudStorageAccount.Parse(_fileStorageConfig.Value.StorageConnectionString);
                                        var client = account.CreateCloudBlobClient();
                                        var container = client.GetContainerReference(_fileStorageConfig.Value.ContainerName);

                                        var applicationFolder = container.GetDirectoryReference(section.ApplicationId.ToString());
                                        var sequenceFolder = applicationFolder.GetDirectoryReference(section.SequenceNo.ToString());
                                        var sectionFolder = sequenceFolder.GetDirectoryReference(section.SectionNo.ToString());
                                        var pageFolder = sectionFolder.GetDirectoryReference(page.PageId.ToLower());

                                        var questionFolder = pageFolder.GetDirectoryReference(answer.QuestionId.ToLower());

                                        var blobReference = questionFolder.GetBlockBlobReference(answer.Value);

                                        if (blobReference.Exists())
                                        {
                                            var newfileurl = $"{section.ApplicationId.ToString().ToLower()}/{sequenceId.ToString().ToLower()}/{sectionId.ToString().ToLower()}/{page.PageId}/{answer.QuestionId.ToLower()}/{answer.Value}";
                                            var newFileLocation = container.GetBlockBlobReference(newfileurl);

                                            await newFileLocation.StartCopyAsync(blobReference);

                                            result.MigratedFiles.Add(new MigratedFile { From = blobReference.Name, To = newfileurl });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Error running file migration: {ex.Message}. Stack trace: {ex.StackTrace}");
                return new FileMigrationResult() { Error = ex.Message, ErrorStackTrace = ex.StackTrace };
            }

            return result;
        }
    }

    public class FileMigrationResult
    {
        public List<MigratedFile> MigratedFiles { get; set; }
        public string Error { get; set; }
        public string ErrorStackTrace { get; set; }
    }

    public class MigratedFile
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}