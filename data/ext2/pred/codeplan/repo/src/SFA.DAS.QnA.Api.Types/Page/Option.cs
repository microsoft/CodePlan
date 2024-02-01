using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Option
    {
        public List<Question> FurtherQuestions { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string HintText { get; set; }
        public bool HasHintText => !string.IsNullOrWhiteSpace(HintText);
        public string ConditionalContentText { get; set; }
        public bool HasConditionalContentText => !string.IsNullOrWhiteSpace(ConditionalContentText);
    }
}