using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetSections;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSectionsTests
{
    public class When_GetSections_handled : GetSectionsTestBase
    {
        [Test]
        public async Task Then_the_correct_sections_are_returned()
        {
            var results = await Handler.Handle(new GetSectionsRequest(ApplicationId), CancellationToken.None);

            results.Value.Count.Should().Be(3);
        }
    }
}
