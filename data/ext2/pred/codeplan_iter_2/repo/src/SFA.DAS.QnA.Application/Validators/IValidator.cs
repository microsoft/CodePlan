using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public interface IValidator
    {
        ValidationDefinition ValidationDefinition { get; set; }
        List<KeyValuePair<string, string>> Validate(Question question, Answer answer);
    }
}