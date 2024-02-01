using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetPageBySectionNoTests
{
    public class When_GetPageBySectionNo_handled : GetPageBySectionNoTestBase
    {
        [Test]
        public async Task Then_the_Page_is_returned()
        {
            var result = await Handler.Handle(new GetPageBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, PageId), CancellationToken.None);

            result.Value.Should().NotBeNull();
        }
    }
}