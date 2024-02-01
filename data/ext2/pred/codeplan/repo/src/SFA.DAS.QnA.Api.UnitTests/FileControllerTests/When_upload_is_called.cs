using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Api.UnitTests.FileControllerTests
{
    [TestFixture]
    public class When_upload_is_called
    {
        private ILogger<FileController> _logger;
        private IMediator _mediator;
        private FileController _fileController;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<FileController>>();
            _mediator = Substitute.For<IMediator>();
            _fileController = new FileController(_logger, _mediator)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Test]
        public async Task Then_bad_request_returned_if_fails_to_upload()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            var pageId = "pageId";
            var failMessage = "failed";

            _mediator.Send(Arg.Any<SubmitPageOfFilesRequest>()).Returns(new HandlerResponse<SetPageAnswersResponse>(false, failMessage));

            var result = await _fileController.Upload(applicationId, sectionId, pageId);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Then_success_returned_if_uploaded_successfully()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            var pageId = "pageId";
            var nextAction = "nextAction";
            var nextActionId = "nextActionId";

            var response = new SetPageAnswersResponse(nextAction, nextActionId);

            _mediator.Send(Arg.Any<SubmitPageOfFilesRequest>()).Returns(new HandlerResponse<SetPageAnswersResponse>(response));

            _fileController.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
            var result = await _fileController.Upload(applicationId, sectionId, pageId);

            result.Value.Should().BeOfType<SetPageAnswersResponse>();
            result.Value.ValidationPassed.Should().BeTrue();
            result.Value.NextAction.Should().Be(nextAction);
            result.Value.NextActionId.Should().Be(nextActionId);
        }
    }
}
