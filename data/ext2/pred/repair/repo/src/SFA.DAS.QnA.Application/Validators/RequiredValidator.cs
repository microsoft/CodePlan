using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class RequiredValidator : IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (text == null || question.Input.GetEmptyAnswerValues().Contains(text))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }
    }
}