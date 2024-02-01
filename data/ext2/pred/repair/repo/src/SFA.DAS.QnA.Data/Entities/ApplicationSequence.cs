using System;
using System.Collections.Generic;

namespace SFA.DAS.QnA.Data.Entities
{
    public class ApplicationSequence
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public bool IsActive { get; set; }
    }
}