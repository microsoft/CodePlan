using System;

namespace SFA.DAS.QnA.Api.Types
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public Guid ProjectId { get; set; }
        public string ApplicationDataSchema { get; set; }
    }
}