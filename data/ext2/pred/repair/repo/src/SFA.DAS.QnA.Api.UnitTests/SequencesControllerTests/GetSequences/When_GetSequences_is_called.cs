using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetSequences
{
    [TestFixture]
    public class When_GetSequences_is_called
    {
        [Test]
        public async Task Then_a_list_of_sequences_is_returned()
        {
            var mediator = Substitute.For<IMediator>();
            var applicationId = Guid.NewGuid();

            mediator.Send(Arg.Any<GetSequencesRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<List<Sequence>>
                {
                    Value = new List<Sequence>
                    {
                        new Sequence(),
                        new Sequence()
                    }
                });

            var controller = new SequencesController(mediator);

            var result = await controller.GetSequences(applicationId);

            result.Value.Should().BeOfType<List<Sequence>>();
            result.Value.Count.Should().Be(2);
        }
    }
}