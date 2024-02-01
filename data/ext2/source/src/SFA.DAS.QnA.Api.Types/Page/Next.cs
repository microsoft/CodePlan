using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Next
    {
        public string Action { get; set; }
        public string ReturnId { get; set; }
        public List<Condition> Conditions { get; set; }
        public bool ConditionMet { get; set; }
    }
}