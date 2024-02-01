using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class EmailTypeValidator : IValidator
    {
        public EmailTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() { ErrorMessage = "Answer must be a valid email" };
        }

        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidEmail(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
