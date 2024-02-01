using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.Files.DeleteFile;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersHandlerTests
{
    public class When_section_found : ResetSectionAnswersTestBase
    {
        [Test]
        public async Task Then_successful_response()
        {
            var response = await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            response.Value.HasSectionAnswersBeenReset.Should().BeTrue();
        }

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        public async Task Then_page_answers_are_reset(string pageId)
        {
            await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, pageId), CancellationToken.None);
            getPageResponse.Value.PageOfAnswers.Should().BeEmpty();
        }

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        public async Task Then_page_complete_is_false(string pageId)
        {
            await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, pageId), CancellationToken.None);
            getPageResponse.Value.Complete.Should().BeFalse();
        }

        [Test]
        public async Task Then_questiontag_is_reset()
        {
            await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            var getApplicationDataResponse = await GetApplicationDataHandler.Handle(new GetApplicationDataRequest(ApplicationId), CancellationToken.None);

            var applicationData = JsonDocument.Parse(getApplicationDataResponse.Value).RootElement;
            var questionTag = applicationData.GetProperty("Q1");

            using (new AssertionScope())
            {
                questionTag.ValueKind.Should().Be(JsonValueKind.Null);
                applicationData.TryGetProperty("Q2", out _).Should().BeFalse();
            }
        }

        [TestCase("1", true)]
        [TestCase("2", false)]
        [TestCase("3", false)]
        public async Task Then_pages_has_active_status_set_correctly(string pageId, bool active)
        {
            await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            var pageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, pageId), CancellationToken.None);
            pageResponse.Value.Active.Should().Be(active);
        }

        [TestCase("2", "Q2", "Folder/Filename.pdf")]
        public async Task Then_delete_file_request_sent_for_file_upload_question(string pageId, string questionId, string fileName)
        {
            await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo), CancellationToken.None);

            await Mediator.Received(1).Send(Arg.Is<DeleteFileRequest>(x => x.ApplicationId == ApplicationId && x.SectionId == SectionId && x.PageId == pageId && x.QuestionId == questionId && x.FileName == fileName));
        }
    }
}
