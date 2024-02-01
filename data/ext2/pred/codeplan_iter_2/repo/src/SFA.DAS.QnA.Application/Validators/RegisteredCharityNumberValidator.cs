using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class RegisteredCharityNumberValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text) && !IsValidRegisteredCharityNumber(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        private static bool IsValidRegisteredCharityNumber(string registeredCharityNumber)
        {
            try
            {
                // MFC 28/01/2019 left in cos specific rules unclear
                //var rx = new Regex(@"^[0-9]{7}$");
                //if (registeredCharityNumber.Length==8)
                //    registeredCharityNumber = registeredCharityNumber.Replace("-","");

                return Regex.IsMatch(registeredCharityNumber, @"^[0-9-]{1,}$");
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
