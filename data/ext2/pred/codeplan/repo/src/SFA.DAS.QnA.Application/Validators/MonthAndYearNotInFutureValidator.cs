using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class MonthAndYearNotInFutureValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();
            var dateParts = answer?.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrEmpty(text) && dateParts != null && dateParts.Length == 2)
            {
                var month = dateParts[0];
                var year = dateParts[1];

                if (string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
                {
                    return errors;
                }

                var dateString = $"1/{month}/{year}";
                var formatStrings = new string[] { "d/M/yyyy" };

                if (DateTime.TryParseExact(dateString, formatStrings, null, DateTimeStyles.None, out var dateEntered) && dateEntered > DateTime.Today)
                {
                    errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                }
            }

            return errors;
        }
    }
}