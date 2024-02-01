using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.ApplicationDataControllerTests
{
    [TestFixture]
    public class WhenGettingApplicationData
    {
        [Test]
        public async Task And_MediatorCallFails_ThenNotFoundResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new ApplicationDataController(mediator);
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>() { Success = false, Message = "Application does not exist." });

            var result = await controller.Get(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task And_MediatorCallIsSuccessful_ThenOkObjectResultReturned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new ApplicationDataController(mediator);
            var applicationData = @"{""OrganisationReferenceId"": ""c3333b62-a07c-415e-8778-84222231b0s1""}";
            mediator.Send(Arg.Any<GetApplicationDataRequest>()).Returns(new HandlerResponse<string>(applicationData));

            var result = await controller.Get(Guid.NewGuid());

            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}