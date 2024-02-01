using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MonthAndYearTypeValidator : IValidator
    {
        public MonthAndYearTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() { ErrorMessage = "Answer must be a valid month and year" };
        }

        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();
            var dateParts = text?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(text) || dateParts is null || dateParts.Length != 2)
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }
            else
            {
                var month = dateParts[0];
                var year = dateParts[1];

                if (string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
                {
                    errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                }
                else
                {
                    var dateString = $"1/{month}/{year}";
                    var formatStrings = new string[] { "d/M/yyyy" };

                    if (!DateTime.TryParseExact(dateString, formatStrings, null, DateTimeStyles.None, out _))
                    {
                        errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                    }
                }
            }

            return errors;
        }
    }
}