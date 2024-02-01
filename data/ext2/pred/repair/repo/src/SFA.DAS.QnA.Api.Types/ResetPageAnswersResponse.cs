using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types
{
    public class ResetPageAnswersResponse
    {
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public bool ValidationPassed { get; set; }
        public bool HasPageAnswersBeenReset { get; set; }

        public ResetPageAnswersResponse()
        { }

        public ResetPageAnswersResponse(bool hasPageAnswersBeenReset)
        {
            ValidationPassed = true;
            HasPageAnswersBeenReset = hasPageAnswersBeenReset;
        }

        public ResetPageAnswersResponse(List<KeyValuePair<string, string>> validationErrors)
        {
            ValidationErrors = validationErrors;
            ValidationPassed = false;
        }
    }
}