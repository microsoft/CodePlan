using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    public class When_files_specified_is_null : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_return_no_files_specified_message()
        {
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "1", null), CancellationToken.None);

            response.Success.Should().BeFalse();
            response.Message.Should().Be("No files specified.");
        }
    }
}
