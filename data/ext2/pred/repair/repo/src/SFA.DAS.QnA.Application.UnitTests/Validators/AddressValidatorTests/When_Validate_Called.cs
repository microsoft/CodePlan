using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.AddressValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", true)]
        [TestCase("{}", false)]
        [TestCase("{\"AddressLine1\" : \"\"}", false)]
        [TestCase("{\"AddressLine1\" : null}", false)]
        [TestCase("{\"AddressLine1\" : \"1 Test Street\"}", true)]
        [TestCase("{\"AddressLine1\" : \"1 Test Street\", \"AddressLine3\" : \"London\", \"Postcode\" : \"AB1 1BA\"}", true)]
        public void Then_BuildingAndStreet_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new AddressBuildingAndStreetRequiredValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "AddressLine1 is required",
                    Name = "AddressBuildingAndStreetRequired"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }

        [TestCase("", true)]
        [TestCase("{}", false)]
        [TestCase("{\"Postcode\" : \"\"}", false)]
        [TestCase("{\"Postcode\" : null}", false)]
        [TestCase("{\"Postcode\" : \"AB1 1BA\"}", true)]
        [TestCase("{\"AddressLine1\" : \"1 Test Street\", \"AddressLine3\" : \"London\", \"Postcode\" : \"AB1 1BA\"}", true)]
        public void Then_Postcode_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new AddressPostcodeRequiredValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Postcode is required",
                    Name = "AddressPostcodeRequired"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }

        [TestCase("", true)]
        [TestCase("{}", false)]
        [TestCase("{\"AddressLine3\" : \"\"}", false)]
        [TestCase("{\"AddressLine3\" : null}", false)]
        [TestCase("{\"AddressLine3\" : \"London\"}", true)]
        [TestCase("{\"AddressLine1\" : \"1 Test Street\", \"AddressLine3\" : \"London\", \"Postcode\" : \"AB1 1BA\"}", true)]
        public void Then_TownOrCity_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new AddressTownOrCityRequiredValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "AddressLine3 is required",
                    Name = "AddressTownOrCityRequired"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
