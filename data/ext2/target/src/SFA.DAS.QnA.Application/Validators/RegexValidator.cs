using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class RegexValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidRegexMatch(text, ValidationDefinition.Value.ToString()))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidRegexMatch(string input, string pattern)
        {
            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}