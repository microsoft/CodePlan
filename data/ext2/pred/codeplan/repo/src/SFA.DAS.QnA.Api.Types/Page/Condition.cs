namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Condition
    {
        public string QuestionId { get; set; }
        public string QuestionTag { get; set; }
        public string MustEqual { get; set; }
        public string Contains { get; set; }
    }
}