using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;


namespace SFA.DAS.QnA.Application.UnitTests.Validators.MonthAndYearValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", false)]
        [TestCase(",", false)]
        [TestCase("01,0001", true)]
        [TestCase("11,2019", true)]
        [TestCase("12,9999", true)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new MonthAndYearValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Not a valid Date",
                    Name = "MonthAndYear"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
