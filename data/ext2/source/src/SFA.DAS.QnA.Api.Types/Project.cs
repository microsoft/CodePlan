using System;

namespace SFA.DAS.QnA.Api.Types
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApplicationDataSchema { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}