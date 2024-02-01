using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.CanUpdateTests
{
    public class When_CanUpdatePage_handled : CanUpdatePageTestBase
    {
        [Test]
        public async Task Then_true_is_returned_for_an_Active_page()
        {
            var result = await Handler.Handle(new CanUpdatePageRequest(ApplicationId, SectionId, ActivePageId), CancellationToken.None);

            result.Value.Should().BeTrue();
        }

        [Test]
        public async Task Then_false_is_returned_for_an_Inactive_page()
        {
            var result = await Handler.Handle(new CanUpdatePageRequest(ApplicationId, SectionId, InactivePageId), CancellationToken.None);

            result.Value.Should().BeFalse();
        }
    }
}