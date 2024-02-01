﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.DateValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("", false)]
        [TestCase(",,", false)]
        [TestCase("01,01,0001", true)]
        [TestCase("01,11,2019", true)]
        [TestCase("31,12,9999", true)]
        [TestCase("32,12,2020", false)]
        [TestCase("31,13,2020", false)]
        [TestCase("29,02,2020", true)]
        [TestCase("29,02,2021", false)]
        public void Then_correct_errors_are_returned(string input, bool isValid)
        {
            var validator = new DateValidator
            {
                ValidationDefinition = new ValidationDefinition()
                {
                    ErrorMessage = "Not a valid Date",
                    Name = "Date"
                }
            };

            var question = new Question { QuestionId = "Q1" };
            var errors = validator.Validate(question, new Answer { Value = input, QuestionId = question.QuestionId });

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
