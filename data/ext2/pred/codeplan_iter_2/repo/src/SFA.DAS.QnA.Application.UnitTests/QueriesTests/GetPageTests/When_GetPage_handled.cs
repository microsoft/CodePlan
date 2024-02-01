using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetPageTests
{
    public class When_GetPage_handled : GetPageTestBase
    {
        [Test]
        public async Task Then_the_Page_is_returned()
        {
            var result = await Handler.Handle(new GetPageRequest(ApplicationId, SectionId, PageId), CancellationToken.None);

            result.Value.Should().NotBeNull();
        }
    }
}