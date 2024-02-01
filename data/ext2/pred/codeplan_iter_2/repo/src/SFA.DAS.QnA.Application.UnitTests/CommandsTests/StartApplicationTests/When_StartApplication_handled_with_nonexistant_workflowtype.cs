using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.StartApplication;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.StartApplicationTests
{
    public class When_StartApplication_handled_with_nonexistant_workflowtype : StartApplicationTestBase
    {
        [Test]
        public async Task Then_success_false_is_returned()
        {
            var result = await Handler.Handle(new StartApplicationRequest() { UserReference = "dave", WorkflowType = "NONEXISTANT_WORKFLOWTYPE" }, CancellationToken.None);

            result.Success.Should().BeFalse();
        }
    }
}