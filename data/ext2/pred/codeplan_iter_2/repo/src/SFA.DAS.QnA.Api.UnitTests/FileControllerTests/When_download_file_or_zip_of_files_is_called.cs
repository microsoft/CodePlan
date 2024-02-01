using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.DownloadFile;

namespace SFA.DAS.QnA.Api.UnitTests.FileControllerTests
{
    [TestFixture]
    public class When_download_file_or_zip_of_files_is_called
    {
        private ILogger<FileController> _logger;
        private IMediator _mediator;
        private FileController _fileController;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<FileController>>(); ;
            _mediator = Substitute.For<IMediator>();
            _fileController = new FileController(_logger, _mediator);
        }

        [Test]
        public async Task Then_the_file_is_returned()
        {
            var expectedApplicationId = Guid.NewGuid();
            var expectedSequenceNo = 123;
            var expectedSectionNo = 456;
            var expectedPageId = "pageId";
            var expectedQuestionId = "questionId";

            var expectedResponse = new DownloadFile
            {
                ContentType = "application/pdf",
                FileName = "fileName",
                Stream = new MemoryStream()
            };

            _mediator.Send(Arg.Is<DownloadFileBySectionNoRequest>(x =>
                    x.ApplicationId == expectedApplicationId && x.SectionNo == expectedSectionNo &&
                    x.SequenceNo == expectedSequenceNo && x.PageId == expectedPageId &&
                    x.QuestionId == expectedQuestionId))
                .Returns(new HandlerResponse<DownloadFile>(expectedResponse));

            var result = await _fileController.DownloadFileOrZipOfFiles(expectedApplicationId, expectedSequenceNo, expectedSectionNo, expectedPageId, expectedQuestionId) as FileStreamResult;

            result.FileStream.Should().BeSameAs(expectedResponse.Stream);
            result.ContentType.Should().Be(expectedResponse.ContentType);
            result.FileDownloadName.Should().Be(expectedResponse.FileName);
        }
    }
}
