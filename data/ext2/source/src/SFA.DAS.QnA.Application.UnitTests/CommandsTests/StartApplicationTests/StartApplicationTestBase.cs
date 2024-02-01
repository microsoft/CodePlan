using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.StartApplication;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.StartApplicationTests
{
    [TestFixture]
    public class StartApplicationTestBase
    {
        protected QnaDataContext DataContext;
        protected StartApplicationHandler Handler;
        protected Guid WorkflowId;

        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();

            var applicationDataValidator = Substitute.For<IApplicationDataValidator>();
            applicationDataValidator.IsValid("", "").ReturnsForAnyArgs(true);
            var logger = Substitute.For<ILogger<StartApplicationHandler>>();
            Handler = new StartApplicationHandler(DataContext, applicationDataValidator, logger);

            WorkflowId = Guid.NewGuid();

            var projectId = Guid.NewGuid();
            await DataContext.Workflows.AddAsync(
                new Workflow() { Type = "EPAO", Status = WorkflowStatus.Live, Id = WorkflowId, ProjectId = projectId });

            var workflowSections = new[]
            {
                new WorkflowSection {Id = Guid.NewGuid(), Title = "Section 1", QnAData = new QnAData(){Pages = new List<Page>()
                {
                    new Page() {Title = "[PageTitleToken1]"},
                    new Page() {Title = "[PageTitleToken2]"}
                }}},
                new WorkflowSection {Id = Guid.NewGuid(), Title = "Section 2", QnAData = new QnAData(){Pages = new List<Page>()}},
                new WorkflowSection {Id = Guid.NewGuid(), Title = "Section 3", QnAData = new QnAData(){Pages = new List<Page>()}},
                new WorkflowSection {Id = Guid.NewGuid(), Title = "Section 4", QnAData = new QnAData(){Pages = new List<Page>()}},
                new WorkflowSection {Id = Guid.NewGuid(), Title = "Invalid section", QnAData = new QnAData(){Pages = new List<Page>()}}
            };

            await DataContext.WorkflowSections.AddRangeAsync(workflowSections);

            await DataContext.WorkflowSequences.AddRangeAsync(new[]
            {
                new WorkflowSequence {WorkflowId = WorkflowId, SectionId = workflowSections[0].Id, SectionNo = 1, SequenceNo = 1, IsActive = true},
                new WorkflowSequence {WorkflowId = WorkflowId, SectionId = workflowSections[1].Id, SectionNo = 2, SequenceNo = 1, IsActive = true},
                new WorkflowSequence {WorkflowId = WorkflowId, SectionId = workflowSections[2].Id, SectionNo = 3, SequenceNo = 1, IsActive = true},
                new WorkflowSequence {WorkflowId = WorkflowId, SectionId = workflowSections[3].Id, SectionNo = 4, SequenceNo = 2, IsActive = false},
                new WorkflowSequence {WorkflowId = Guid.NewGuid()},
                new WorkflowSequence {WorkflowId = Guid.NewGuid()},
            });

            await DataContext.Projects.AddAsync(new Project { Id = projectId, ApplicationDataSchema = "" });

            await DataContext.SaveChangesAsync();
        }
    }
}