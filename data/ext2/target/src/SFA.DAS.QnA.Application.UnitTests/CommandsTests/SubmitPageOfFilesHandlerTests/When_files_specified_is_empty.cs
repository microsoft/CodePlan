using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    public class When_files_specified_is_empty : SubmitPageOfFilesTestBase
    {
        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_validation_passes()
        {
            var files = new FormFileCollection();
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "1", files), CancellationToken.None);

            response.Value.ValidationPassed.Should().BeTrue();
        }
    }
}
