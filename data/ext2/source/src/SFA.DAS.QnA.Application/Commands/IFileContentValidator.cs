using System.Collections.Generic;
using SFA.DAS.QnA.Api.Types.Page;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.QnA.Application.Commands
{
    public interface IFileContentValidator
    {
        List<KeyValuePair<string, string>> Validate(IFormFileCollection files);
    }
}