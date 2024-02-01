using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.FileTypeValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", "pdf", true)]
        [TestCase("test.txt", "pdf", false)]
        [TestCase("test.pdf.txt", "pdf", false)]
        [TestCase("test.pdf", "pdf", true)]
        [TestCase("test.txt.pdf", "pdf", true)]
        [TestCase("    Mary  had   a   little lamb .pdf", "pdf", true)]
        [TestCase("Mary had a little lamb, its fleece was white as snow.pdf", "pdf", true)]
        public void Then_correct_errors_are_returned(string input, string filetype, bool isValid)
        {
            var validator = new FileTypeValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Incorrect FileType",
                    Name = "FileType",
                    Value = filetype
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
