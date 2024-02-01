namespace SFA.DAS.QnA.Api.Types.Page
{
    public class NotRequiredCondition
    {
        public string Field { get; set; }
        public string[] IsOneOf { get; set; }

        public string[] ContainsAllOf { get; set; }

        public string[] DoesNotContain { get; set; }
    }
}