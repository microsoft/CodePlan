using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;

namespace SFA.DAS.QnA.Application.Commands
{
    public interface IAnswerValidator
    {
        List<KeyValuePair<string, string>> Validate(List<Answer> answers, Page page);
    }
}