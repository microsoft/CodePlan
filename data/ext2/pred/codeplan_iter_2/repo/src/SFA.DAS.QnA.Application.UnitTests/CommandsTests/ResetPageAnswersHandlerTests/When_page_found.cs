
namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersHandlerTests
{
    using System.Text.Json.Nodes;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
    using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;
    using SFA.DAS.QnA.Application.Queries.Sections.GetPage;

    public class When_page_found : ResetPageAnswersTestBase

    {
        [Test]
        public async Task Then_successful_response()
        {
            var response = await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            response.Value.HasPageAnswersBeenReset.Should().BeTrue();
        }

        [Test]
        public async Task Then_page_answers_are_reset()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.PageOfAnswers.Should().BeEmpty();
        }

        [Test]
        public async Task Then_page_complete_is_false()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getPageResponse = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            getPageResponse.Value.Complete.Should().BeFalse();
        }

        [Test]
        public async Task Then_questiontag_is_reset()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var getApplicationDataResponse = await GetApplicationDataHandler.Handle(new GetApplicationDataRequest(ApplicationId), CancellationToken.None);

            var applicationData = JsonNode.Parse(getApplicationDataResponse.Value);
            var questionTag = applicationData.AsObject()["Q1"];

            questionTag.Should().NotBeNull();
            questionTag.Value<string>().Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Then_all_pages_have_their_active_status_set_correctly()
        {
            await Handler.Handle(new ResetPageAnswersRequest(ApplicationId, SectionId, "1"), CancellationToken.None);

            var page1Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "1"), CancellationToken.None);
            page1Response.Value.Active.Should().BeTrue();

            var page2Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "2"), CancellationToken.None);
            page2Response.Value.Active.Should().BeFalse();

            var page3Response = await GetPageHandler.Handle(new GetPageRequest(ApplicationId, SectionId, "3"), CancellationToken.None);
            page3Response.Value.Active.Should().BeFalse();
        }

    }
}