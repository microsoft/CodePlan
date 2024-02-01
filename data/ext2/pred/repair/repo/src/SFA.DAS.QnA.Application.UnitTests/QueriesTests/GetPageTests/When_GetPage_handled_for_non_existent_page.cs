using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetPageTests
{
    public class When_GetPage_handled_for_non_existent_page : GetPageTestBase
    {
        [Test]
        public async Task Then_unsuccessful_response_is_returned()
        {
            var result = await Handler.Handle(new GetPageRequest(ApplicationId, SectionId, Guid.NewGuid().ToString()), CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Value.Should().BeNull();
        }
    }
}