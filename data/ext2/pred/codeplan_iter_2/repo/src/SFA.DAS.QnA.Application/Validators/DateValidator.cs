using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class DateValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }

        public virtual List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            if (IsEmpty(answer, out string[] dateParts) || !TryParseExact(dateParts, out _))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }

        protected bool IsEmpty(Answer answer, out string[] dateParts)
        {
            var text = answer?.Value?.Trim();
            dateParts = text?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return string.IsNullOrEmpty(text) || dateParts is null || dateParts.Length != 3;
        }

        protected bool TryParseExact(string[] dateParts, out DateTime result)
        {
            var day = dateParts[0];
            var month = dateParts[1];
            var year = dateParts[2];

            if (string.IsNullOrWhiteSpace(day) || string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
            {
                result = DateTime.MinValue;
                return false;
            }
            else
            {
                var dateString = $"{day}/{month}/{year}";
                var formatStrings = new string[] { "d/M/yyyy" };

                return DateTime.TryParseExact(dateString, formatStrings, null, DateTimeStyles.None, out result);
            }
        }
    }
}