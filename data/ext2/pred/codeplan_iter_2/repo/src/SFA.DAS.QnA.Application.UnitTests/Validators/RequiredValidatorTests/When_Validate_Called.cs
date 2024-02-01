using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.RequiredValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", "Text", false)]
        [TestCase("     ", "Text", false)]
        [TestCase(".", "Text", true)]
        [TestCase(",", "Text", true)]
        [TestCase(",,", "Text", true)]
        [TestCase("some input", "Text", true)]
        [TestCase("    some input", "Text", true)]
        [TestCase("", "Date", false)]
        [TestCase("     ", "Date", false)]
        [TestCase(",,", "Date", false)]
        [TestCase("10,10,2010", "Date", true)]
        [TestCase(",10,2010", "Date", true)]
        [TestCase("10,,2010", "Date", true)]
        [TestCase("10,10,", "Date", true)]
        [TestCase("", "MonthAndYear", false)]
        [TestCase("     ", "MonthAndYear", false)]
        [TestCase(",", "MonthAndYear", false)]
        [TestCase("10,2010", "MonthAndYear", true)]
        [TestCase(",2010", "MonthAndYear", true)]
        [TestCase("10,", "MonthAndYear", true)]
        public void Then_correct_errors_are_returned(string input, string inputType, bool isValid)
        {
            var validator = new RequiredValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Input is required",
                    Name = "Required"
                }
            };

            var question = new Question { QuestionId = "Q1", Input = new Input { Type = inputType } };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
