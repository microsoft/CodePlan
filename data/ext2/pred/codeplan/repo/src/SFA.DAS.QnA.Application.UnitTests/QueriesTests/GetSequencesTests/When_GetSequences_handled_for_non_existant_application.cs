using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSequencesTests
{
    [TestFixture]
    public class When_GetSequences_handled_for_non_existant_application : GetSequencesTestBase
    {
        private HandlerResponse<List<Sequence>> _results;

        [SetUp]
        public async Task Act()
        {
            _results = await Handler.Handle(new GetSequencesRequest(Guid.NewGuid()), CancellationToken.None);
        }

        [Test]
        public void Then_no_sequences_are_returned()
        {
            _results.Value.Should().BeNull();
        }

        [Test]
        public void Then_success_is_false()
        {
            _results.Success.Should().BeFalse();
        }
    }
}