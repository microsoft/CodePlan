using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Input
    {
        public string Type { get; set; }
        public string InputClasses { get; set; }
        public string InputPrefix { get; set; }
        public string InputSuffix { get; set; }
        public List<Option> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
        public string DataEndpoint { get; set; }

        public List<string> GetEmptyAnswerValues()
        {
            List<string> emptyAnswerValues = new List<string> { string.Empty };

            switch (Type)
            {
                case "MonthAndYear":
                    emptyAnswerValues.Add(",");
                    break;
                case "Date":
                    emptyAnswerValues.Add(",,");
                    break;
            }

            return emptyAnswerValues;
        }
    }
}