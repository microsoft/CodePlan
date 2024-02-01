using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types
{
    public class ResetSectionAnswersResponse
    {
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public bool ValidationPassed { get; set; }
        public bool HasSectionAnswersBeenReset { get; set; }

        public ResetSectionAnswersResponse()
        { }

        public ResetSectionAnswersResponse(bool hasSectionAnswersBeenReset)
        {
            ValidationPassed = true;
            HasSectionAnswersBeenReset = hasSectionAnswersBeenReset;
        }

        public ResetSectionAnswersResponse(List<KeyValuePair<string, string>> validationErrors)
        {
            ValidationErrors = validationErrors;
            ValidationPassed = false;
        }
    }
}