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
    public class When_GetSequence_is_called_with_a_sectionNo
    {
        [Test]
        public async Task And_Sequence_exists_Then_Sequence_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var sequenceNo = 1;

            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<GetSequenceBySequenceNoRequest>()).Returns(new HandlerResponse<Sequence>(new Sequence()
            {
                Id = Guid.NewGuid(),
                SequenceNo = sequenceNo,
                ApplicationId = applicationId
            }));

            var sequenceController = new SequencesController(mediator);

            var result = await sequenceController.GetSequenceBySequenceNo(applicationId, sequenceNo);

            result.Value.Should().BeOfType<Sequence>();
            result.Value.SequenceNo.Should().Be(sequenceNo);
        }
    }
}