using System;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class FinancialApplicationGrade
    {
        public string SelectedGrade { get; set; }
        public string InadequateMoreInformation { get; set; }
        public string GradedBy { get; set; }
        public DateTime GradedDateTime { get; set; }
        public DateTime? FinancialDueDate { get; set; }
    }
}