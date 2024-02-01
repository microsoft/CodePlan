namespace SFA.DAS.QnA.Api.Types
{
    public class SkipPageResponse
    {
        public string NextAction { get; set; }

        public string NextActionId { get; set; }

        public SkipPageResponse()
        { }

        public SkipPageResponse(string nextAction, string nextActionId)
        {
            NextAction = nextAction;
            NextActionId = nextActionId;
        }
    }
}
