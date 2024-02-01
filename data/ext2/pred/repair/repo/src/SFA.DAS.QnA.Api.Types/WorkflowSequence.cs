using System;

namespace SFA.DAS.QnA.Api.Types
{
    public class WorkflowSequence
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public int SequenceNo { get; set; }
        public int SectionNo { get; set; }
        public Guid SectionId { get; set; }
        public bool IsActive { get; set; }
    }
}