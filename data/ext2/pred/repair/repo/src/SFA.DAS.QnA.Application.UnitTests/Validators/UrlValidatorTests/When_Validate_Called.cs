using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.UrlValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", true)]
        [TestCase("google", false)]
        [TestCase("google.co.uk", true)]
        [TestCase("google.com", true)]
        [TestCase("www.google.co.uk", true)]
        [TestCase("www.google.com", true)]
        [TestCase("//www.google.com", false)]
        [TestCase("://www.google.com", false)]
        [TestCase("http://www.google.co.uk", true)]
        [TestCase("http://www.google.com", true)]
        [TestCase("https://www.google.co.uk", true)]
        [TestCase("https://www.google.com", true)]
        [TestCase("ftp://www.google.co.uk", true)]
        [TestCase("ftp://www.google.com", true)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new UrlValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Not a valid Url",
                    Name = "Url"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
