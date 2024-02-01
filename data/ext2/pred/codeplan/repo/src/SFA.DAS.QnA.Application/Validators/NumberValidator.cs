using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.QnA.Application.Validators
{
    public class NumberValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidNumber(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidNumber(string number)
        {
            var isValid = long.TryParse(number, out var _);

            if (!isValid)
            {
                try
                {
                    // Fall back to RegEx in case it's a huge positive/negative number
                    isValid = Regex.IsMatch(number, @"^[+-]?[\d]*$");
                }
                catch (ArgumentException)
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}