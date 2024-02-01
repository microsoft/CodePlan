namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Api.Types.Page;
    using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

    public class When_page_not_found : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "NOT_FOUND", new List<Answer>
            {
                new Answer { QuestionId = "Q1", Value = "NOT_FOUND" }
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
