using System;
using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class DateInFutureValidator : DateValidator, IValidator
    {
        public override List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();

            if (!IsEmpty(answer, out string[] dateParts) && (!TryParseExact(dateParts, out DateTime dateEntered) || dateEntered <= DateTime.Today))
            {
                errors.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }
    }
}
