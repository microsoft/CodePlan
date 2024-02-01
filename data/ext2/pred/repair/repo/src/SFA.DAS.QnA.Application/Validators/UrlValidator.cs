using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.QnA.Application.Validators
{
    public class UrlValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidUrl(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidUrl(string url)
        {
            bool isValid;

            try
            {
                // This is intended for HTTPS & HTTP protocol only
                isValid = Regex.IsMatch(url,
                    @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
            }
            catch (FormatException)
            {
                isValid = false;
            }

            if (!isValid)
            {
                // This is backup plan, but only validate against an Absolute Uri!
                isValid = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            }

            return isValid;
        }
    }
}