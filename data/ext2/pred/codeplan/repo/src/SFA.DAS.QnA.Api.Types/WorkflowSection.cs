using System;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Api.Types
{
    public class WorkflowSection
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }
        public QnAData QnAData { get; set; }
    }
}