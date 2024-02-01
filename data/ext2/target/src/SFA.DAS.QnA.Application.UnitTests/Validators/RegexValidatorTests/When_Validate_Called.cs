using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;
namespace SFA.DAS.QnA.Application.UnitTests.Validators.RegexValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", "", true)]
        [TestCase("", @"^[\d]*$", true)]
        [TestCase("Must contain digits", @"^[\d]*$", false)]
        [TestCase("12345", @"^[\d]*$", true)]
        [TestCase("Invalid regex", @"^[", false)]
        public void Then_correct_errors_are_returned(string input, string regex, bool isValid)
        {
            var validator = new RegexValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Regex match failed",
                    Name = "Regex",
                    Value = regex
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
