using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class AddressBuildingAndStreetRequiredValidator : AddressRequiredValidatorBase, IValidator
    {
        public ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            var text = answer?.Value?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var validateErrors = ValidateProperty(question.QuestionId, text, "AddressLine1", ValidationDefinition.ErrorMessage);

                if (validateErrors.Count > 0)
                {
                    errors.AddRange(validateErrors);
                }
            }

            return errors;
        }
    }
}