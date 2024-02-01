using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types
{
    public class SetPageAnswersResponse
    {
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public bool ValidationPassed { get; set; }
        public string NextAction { get; set; }

        public string NextActionId { get; set; }

        public SetPageAnswersResponse()
        { }

        public SetPageAnswersResponse(string nextAction, string nextActionId)
        {
            ValidationPassed = true;
            NextAction = nextAction;
            NextActionId = nextActionId;
        }

        public SetPageAnswersResponse(List<KeyValuePair<string, string>> validationErrors)
        {
            ValidationErrors = validationErrors;
            ValidationPassed = false;
        }
    }
}