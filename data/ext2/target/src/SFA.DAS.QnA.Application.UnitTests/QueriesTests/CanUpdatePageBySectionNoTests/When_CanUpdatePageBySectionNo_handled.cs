using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.CanUpdatePageBySectionNoTests
{
    public class When_CanUpdatePageBySectionNo_handled : CanUpdatePageBySectionNoTestBase
    {
        [Test]
        public async Task Then_true_is_returned_for_an_Active_page()
        {
            var result = await Handler.Handle(new CanUpdatePageBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, ActivePageId), CancellationToken.None);

            result.Value.Should().BeTrue();
        }

        [Test]
        public async Task Then_false_is_returned_for_an_Inactive_page()
        {
            var result = await Handler.Handle(new CanUpdatePageBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, InactivePageId), CancellationToken.None);

            result.Value.Should().BeFalse();
        }
    }
}