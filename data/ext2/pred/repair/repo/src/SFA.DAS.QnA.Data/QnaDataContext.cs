using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.QnA.Data
{
    public class QnaDataContext : DbContext
    {
        public QnaDataContext(DbContextOptions<QnaDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationSection>()
                .Property(c => c.QnAData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v,
                        new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => JsonSerializer.Deserialize<QnAData>(v,
                        new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));

            modelBuilder.Entity<WorkflowSection>()
                .Property(c => c.QnAData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v,
                        new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => JsonSerializer.Deserialize<QnAData>(v,
                        new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
        }

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowSection> WorkflowSections { get; set; }
        public DbSet<WorkflowSequence> WorkflowSequences { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSequence> ApplicationSequences { get; set; }
        public DbSet<ApplicationSection> ApplicationSections { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}