using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    public class When_file_doesnt_relate_to_a_question : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "1", new FormFileCollection
            {
                GenerateFile("This is a dummy file", null, "File.txt")
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
