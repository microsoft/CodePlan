using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetSequences
{
    [TestFixture]
    public class When_GetSequences_called_for_non_existant_application
    {
        [Test]
        public async Task Then_NotFound_returned()
        {
            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<GetSequencesRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<List<Sequence>> { Success = false, Message = "Application does not exist" });

            var controller = new SequencesController(mediator);

            var result = await controller.GetSequences(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}