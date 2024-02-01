using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Application.Commands.ResetPagesToIncomplete;

namespace SFA.DAS.QnA.Api.UnitTests.PagesControllerTests
{
    public class PagesControllerTests
    {
        private Mock<IMediator> _mediator;
        private PagesController _controller;
        private Guid _applicationId;

        [SetUp]
        public void TestSetup()
        {
            _applicationId = Guid.NewGuid();
            _mediator = new Mock<IMediator>();
            _controller = new PagesController(Mock.Of<ILogger<PagesController>>(), _mediator.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Test]
        public async Task SetPagesToIncomplete_HandlerIsCalled()
        {
            const int sequenceNo = 1;
            const int sectionNo = 5;
            var pageIdsToExclude = new List<string>();

            _mediator.Setup(x => x.Send(It.Is<ResetPagesToIncompleteRequest>(y => y.ApplicationId == _applicationId && y.SequenceNo == sequenceNo && y.SectionNo == sectionNo), It.IsAny<CancellationToken>()));

            await _controller.SetPagesToIncomplete(_applicationId, sequenceNo, sectionNo, pageIdsToExclude);

            _mediator.Verify(x => x.Send(It.Is<ResetPagesToIncompleteRequest>(y => y.ApplicationId == _applicationId && y.SequenceNo == sequenceNo && y.SectionNo == sectionNo), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
