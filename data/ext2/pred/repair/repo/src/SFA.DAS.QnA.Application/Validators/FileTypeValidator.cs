using System;
using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class FileTypeValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            var allowedExtension = ValidationDefinition.Value?.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[0];

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(allowedExtension))
            {
                var fileNameParts = text.Split(".", StringSplitOptions.RemoveEmptyEntries);
                var fileNameExtension = fileNameParts[fileNameParts.Length - 1];

                if (!fileNameExtension.Equals(allowedExtension, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                }
            }

            return errors;
        }
    }
}