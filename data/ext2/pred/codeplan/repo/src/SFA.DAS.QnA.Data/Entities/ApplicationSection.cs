using System;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationSection
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public int SectionNo { get; set; }
        public QnAData QnAData { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }
        public Guid SequenceId { get; set; }
    }
}