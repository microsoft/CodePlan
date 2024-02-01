using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class QnAData
    {
        public QnAData() { }

        public QnAData(QnAData copyFrom)
        {
            this.Pages = copyFrom.Pages;
        }

        public bool? RequestedFeedbackAnswered => (Pages is null || Pages.All(p => !p.HasFeedback)) ? null : (bool?)Pages.All(p => p.AllFeedbackIsCompleted);
        public List<Page> Pages { get; set; }
    }
}