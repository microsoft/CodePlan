using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types
{
    public class AddPageAnswerResponse
    {
        public Page.Page Page { get; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; }

        public AddPageAnswerResponse(List<KeyValuePair<string, string>> validationErrors)
        {
            ValidationErrors = validationErrors;
            Success = false;
        }

        public AddPageAnswerResponse(Page.Page page)
        {
            Page = page;
            Success = true;
        }

        public bool Success { get; set; }
    }
}