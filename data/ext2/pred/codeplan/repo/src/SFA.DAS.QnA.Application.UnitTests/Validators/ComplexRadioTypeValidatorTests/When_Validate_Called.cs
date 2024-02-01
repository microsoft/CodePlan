using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;
using System.Collections.Generic;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.ComplexRadioTypeValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", true)]
        [TestCase("yes", true)]
        [TestCase("no", true)]
        [TestCase("invalid option", false)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new ComplexRadioTypeValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Answer must be one of the Input Options",
                    Name = "ComplexRadio"
                }
            };

            var options = new List<Option> { new Option { Value = "yes" }, new Option { Value = "no" } };
            var question = new Question { QuestionId = "Q1", Input = new Input { Options = options } };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
