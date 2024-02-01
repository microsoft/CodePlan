using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.DateTypeValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", false)]
        [TestCase(",,", false)]
        [TestCase("01,01,0001", true)]
        [TestCase("01,11,2019", true)]
        [TestCase("31,12,9999", true)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new DateTypeValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Answer must be a valid date",
                    Name = "Date"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
