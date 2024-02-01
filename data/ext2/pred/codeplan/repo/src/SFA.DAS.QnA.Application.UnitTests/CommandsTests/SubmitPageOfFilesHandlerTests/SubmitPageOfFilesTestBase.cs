using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;
using SFA.DAS.QnA.Application.Commands;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Application.Commands.Files;
using System.IO;
using SFA.DAS.QnA.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    [TestFixture]
    public class SubmitPageOfFilesTestBase
    {
        private const string _fileStorageConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
        private string _fileStorageContainerName = Guid.NewGuid().ToString();

        protected Guid ApplicationId;
        protected Guid SectionId;
        protected SubmitPageOfFilesHandler Handler;
        protected QnaDataContext DataContext;
        protected NotRequiredProcessor NotRequiredProcessor;
        protected TagProcessingService TagProcessingService;

        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            NotRequiredProcessor = new NotRequiredProcessor();
            TagProcessingService = new TagProcessingService(DataContext);
            var fileStorageConfig = GetFileStorageConfig();

            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.Encrypt(Arg.Any<Stream>()).Returns(callinfo => callinfo.ArgAt<Stream>(0)); // Don't Encrypt stream
            encryptionService.Decrypt(Arg.Any<Stream>()).Returns(callinfo => callinfo.ArgAt<Stream>(0)); // Don't Decrypt stream

            var validator = Substitute.For<IAnswerValidator>();
            validator.Validate(Arg.Any<List<Answer>>(), Arg.Any<Page>()).Returns(new List<KeyValuePair<string, string>>());

            var fileContentValidator = Substitute.For<IFileContentValidator>();
            fileContentValidator.Validate(Arg.Any<IFormFileCollection>()).Returns(new List<KeyValuePair<string, string>>());

            Handler = new SubmitPageOfFilesHandler(DataContext, fileStorageConfig, encryptionService, validator, fileContentValidator, NotRequiredProcessor, TagProcessingService);

            ApplicationId = Guid.NewGuid();
            SectionId = Guid.NewGuid();
            await DataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = ApplicationId,
                Id = SectionId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page()
                        {
                            PageId = "1",
                            Questions = new List<Question>{new Question(){QuestionId = "Q1", Input = new Input { Type = "FileUpload" } }},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>{ new Next { } },
                            Active = true
                        }
                    }
                }
            });

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = ApplicationId, ApplicationData = "{}" });

            await DataContext.SaveChangesAsync();
        }

        private IOptions<FileStorageConfig> GetFileStorageConfig()
        {
            return Options.Create(new FileStorageConfig { ContainerName = _fileStorageContainerName, StorageConnectionString = _fileStorageConnectionString });
        }

        protected static FormFile GenerateFile(string content, string questionId, string filename)
        {
            return new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, 0, questionId, filename)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }
    }
}
