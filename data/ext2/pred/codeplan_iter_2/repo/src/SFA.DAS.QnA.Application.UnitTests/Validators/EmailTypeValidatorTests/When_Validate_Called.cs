using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.EmailTypeValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", true)]
        [TestCase("test", false)]
        [TestCase("test@test", false)]
        [TestCase("test@test      .com", false)]
        [TestCase("test@test.com", true)]
        [TestCase("test@test.co.uk", true)]
        [TestCase("test.test@test.com", true)]
        [TestCase("test-test@test.com", true)]
        [TestCase("test.test@test.co.uk", true)]
        [TestCase("test-test@test.co.uk", true)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new EmailTypeValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Answer must be a valid email",
                    Name = "Email"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
