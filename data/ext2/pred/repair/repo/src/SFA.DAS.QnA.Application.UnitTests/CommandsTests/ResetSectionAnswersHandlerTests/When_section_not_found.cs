namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersHandlerTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;

    public class When_section_not_found : ResetSectionAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var response = await Handler.Handle(new ResetSectionAnswersRequest(ApplicationId, SequenceNo, SectionNo - 1), CancellationToken.None);

            response.Success.Should().BeFalse();
        }
    }
}
