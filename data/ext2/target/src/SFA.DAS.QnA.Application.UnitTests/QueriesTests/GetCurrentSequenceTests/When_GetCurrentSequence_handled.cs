using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetCurrentSequenceTests
{
    public class When_GetCurrentSequence_handled_for_valid_applicationId : GetCurrentSequenceTestBase
    {
        [Test]
        public async Task Then_the_current_sequence_is_returned()
        {
            var results = await Handler.Handle(new GetCurrentSequenceRequest(ApplicationId), CancellationToken.None);

            results.Value.SequenceNo.Should().Be(2);
            results.Value.ApplicationId.Should().Be(ApplicationId);
        }
    }
}