namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    using SFA.DAS.QnA.Application.Commands.Files.UploadFile;
    using System.Threading;
    using System.Threading.Tasks;

    public class When_page_not_found : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new SubmitPageOfFilesRequest(ApplicationId, SectionId, "NOT_FOUND", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "File.txt")
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
