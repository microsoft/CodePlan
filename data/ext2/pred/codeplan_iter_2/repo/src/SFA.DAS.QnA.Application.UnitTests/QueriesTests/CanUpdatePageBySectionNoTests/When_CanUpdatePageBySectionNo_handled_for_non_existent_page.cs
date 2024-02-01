using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.CanUpdatePageBySectionNoTests
{
    public class When_CanUpdatePageBySectionNo_handled_for_non_existent_page : CanUpdatePageBySectionNoTestBase
    {
        [Test]
        public async Task Then_unsuccessful_response_is_returned()
        {
            var result = await Handler.Handle(new CanUpdatePageBySectionNoRequest(ApplicationId, SequenceNo, SectionNo, Guid.NewGuid().ToString()), CancellationToken.None);

            result.Success.Should().BeFalse();
        }
    }
}