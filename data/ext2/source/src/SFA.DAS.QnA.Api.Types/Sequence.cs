using System;

namespace SFA.DAS.QnA.Api.Types
{
    public class Sequence
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public bool IsActive { get; set; }

        public bool NotRequired { get; set; }
    }
}