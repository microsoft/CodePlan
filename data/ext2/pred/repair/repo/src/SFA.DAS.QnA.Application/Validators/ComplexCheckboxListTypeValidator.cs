using System.Collections.Generic;
using System.Linq;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class ComplexChecklistBoxTypeValidator : IValidator
    {
        public ComplexChecklistBoxTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() { ErrorMessage = "Answer must be one of the Input Options" };
        }

        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var validValues = question.Input.Options.Select(o => o.Value).ToList();

                if (validValues.All(v => v != text))
                {
                    errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                }
            }

            return errors;
        }
    }
}
