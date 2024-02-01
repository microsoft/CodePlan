using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetCurrentSequenceTests
{
    public class When_GetCurrentSequence_handled_for_non_existent_applicationId : GetCurrentSequenceTestBase
    {
        [Test]
        public async Task Then_the_current_sequence_is_returned()
        {
            var results = await Handler.Handle(new GetCurrentSequenceRequest(Guid.NewGuid()), CancellationToken.None);

            results.Value.Should().BeNull();
        }
    }
}