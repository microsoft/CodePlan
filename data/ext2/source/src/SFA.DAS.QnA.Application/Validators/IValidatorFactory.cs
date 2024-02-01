using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public interface IValidatorFactory
    {
        List<IValidator> Build(Question question);
    }
}