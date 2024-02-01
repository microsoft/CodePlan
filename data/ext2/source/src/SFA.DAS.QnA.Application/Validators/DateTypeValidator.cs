using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class DateTypeValidator : DateValidator, IValidator
    {
        public DateTypeValidator()
        {
            ValidationDefinition = new ValidationDefinition() { ErrorMessage = "Answer must be a valid date" };
        }

        public override List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            if (IsEmpty(answer, out string[] dateParts) || !TryParseExact(dateParts, out _))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }
    }
}