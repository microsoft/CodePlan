using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sections.GetSections;


namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSectionsTests
{
    [TestFixture]
    public class When_GetSections_handled_for_non_existant_applicationn : GetSectionsTestBase
    {
        private HandlerResponse<List<Section>> _results;

        [SetUp]
        public async Task Act()
        {
            _results = await Handler.Handle(new GetSectionsRequest(Guid.NewGuid()), CancellationToken.None);
        }

        [Test]
        public void Then_no_sections_are_returned()
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
