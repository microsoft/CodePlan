namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Api.Types.Page;
    using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

    public class When_answer_doesnt_relate_to_a_question : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
                new Answer { QuestionId = null, Value = "Yes" }
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
