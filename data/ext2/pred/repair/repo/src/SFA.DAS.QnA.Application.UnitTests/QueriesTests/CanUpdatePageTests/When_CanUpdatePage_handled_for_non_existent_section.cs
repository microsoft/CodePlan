using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.CanUpdateTests
{
    public class When_CanUpdatePage_handled_for_non_existent_section : CanUpdatePageTestBase
    {
        [Test]
        public async Task Then_unsuccessful_response_is_returned()
        {
            var result = await Handler.Handle(new CanUpdatePageRequest(ApplicationId, Guid.NewGuid(), ActivePageId), CancellationToken.None);

            result.Success.Should().BeFalse();
        }
    }
}