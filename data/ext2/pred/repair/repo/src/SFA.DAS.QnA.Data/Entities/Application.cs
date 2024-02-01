using System;

namespace SFA.DAS.QnA.Data.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ApplicationStatus { get; set; }
        public string ApplicationData { get; set; }
    }
}