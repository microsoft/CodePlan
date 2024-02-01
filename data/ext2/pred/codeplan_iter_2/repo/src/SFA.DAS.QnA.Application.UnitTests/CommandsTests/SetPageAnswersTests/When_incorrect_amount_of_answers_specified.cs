namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Api.Types.Page;
    using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

    public class When_incorrect_amount_of_answers_specified : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs_if_no_answers_specified()
        {
            var response = await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", null), CancellationToken.None);

            response.Success.Should().BeFalse();
        }

        [Test]
        public async Task Then_validation_error_occurs_if_less_than_expected_answers_specified()
        {
            var response = await Handler.Handle(new SetPageAnswersRequest(ApplicationId, SectionId, "1", new List<Answer>
            {
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
