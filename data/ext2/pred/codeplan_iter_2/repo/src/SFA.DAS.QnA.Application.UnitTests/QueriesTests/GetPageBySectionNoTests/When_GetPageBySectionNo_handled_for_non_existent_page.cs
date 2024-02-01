using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetPageBySectionNoTests
{
    public class When_GetPageBySectionNo_handled_for_non_existent_page : GetPageBySectionNoTestBase
    {
        [Test]
        public async Task Then_unsuccessful_response_is_returned()
        {
            var result = await Handler.Handle(new GetPageBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, Guid.NewGuid().ToString()), CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Value.Should().BeNull();
        }
    }
}