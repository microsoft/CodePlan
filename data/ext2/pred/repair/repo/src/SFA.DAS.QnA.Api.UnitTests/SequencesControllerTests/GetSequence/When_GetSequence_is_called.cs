using FluentAssertions;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequence;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.UnitTests.SequenceControllerTests.GetSequence
{
    [TestFixture]
    public class When_GetSequence_is_called
    {
        [Test]
        public async Task And_Sequence_exists_Then_Sequence_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var sequenceId = Guid.NewGuid();

            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<GetSequenceRequest>()).Returns(new HandlerResponse<Sequence>(new Sequence()
            {
                Id = sequenceId,
                SequenceNo = 1,
                ApplicationId = applicationId
            }));

            var sequenceController = new SequencesController(mediator);

            var result = await sequenceController.GetSequence(applicationId, sequenceId);

            result.Value.Should().BeOfType<Sequence>();
            result.Value.Id.Should().Be(sequenceId);
        }
    }
}