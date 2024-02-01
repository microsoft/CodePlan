using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.NumberValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", true)]
        [TestCase("one", false)]
        [TestCase("invalid", false)]
        [TestCase("pi", false)]
        [TestCase("infinity", false)]
        [TestCase("NaN", false)]
        [TestCase("-1", true)]
        [TestCase("0", true)]
        [TestCase("1", true)]
        [TestCase(byte.MinValue, true)]
        [TestCase(byte.MaxValue, true)]
        [TestCase(int.MinValue, true)]
        [TestCase(int.MaxValue, true)]
        [TestCase(long.MinValue, true)]
        [TestCase(long.MaxValue, true)]
        [TestCase(ulong.MinValue, true)]
        [TestCase(ulong.MaxValue, true)]
        [TestCase("-987654321012345678987654321", true)]
        [TestCase("987654321012345678987654321", true)]
        public void Then_correct_errors_are_returned(object input, bool isValid)
        {
            var validator = new NumberValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Not a valid Number",
                    Name = "Number"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input?.ToString(), QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
