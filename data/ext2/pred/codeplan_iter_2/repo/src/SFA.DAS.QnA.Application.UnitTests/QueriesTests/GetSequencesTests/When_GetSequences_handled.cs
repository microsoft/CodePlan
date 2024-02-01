using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSequencesTests
{
    public class When_GetSequences_handled : GetSequencesTestBase
    {
        [Test]
        public async Task Then_the_correct_sequences_are_returned()
        {
            var results = await Handler.Handle(new GetSequencesRequest(ApplicationId), CancellationToken.None);

            results.Value.Count.Should().Be(2);
        }
    }
}