using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Workflows.CreateWorkflow;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.Workflows.CreateWorkflowTests
{
    [TestFixture]
    public class When_workflow_is_created
    {
        [Test]
        public async Task Then_ApplicationDataSchema_is_copied_from_Project_to_Workflow()
        {
            var inMemoryDataContext = DataContextHelpers.GetInMemoryDataContext();

            var projectId = Guid.NewGuid();

            var project = new Project
            {
                ApplicationDataSchema = "schema{}",
                Id = projectId
            };

            await inMemoryDataContext.Projects.AddAsync(project);
            await inMemoryDataContext.SaveChangesAsync();

            var createWorkflowHandler = new CreateWorkflowHandler(inMemoryDataContext);
            await createWorkflowHandler.Handle(new CreateWorkflowRequest(projectId, new Workflow() { ProjectId = projectId, Description = "Desc", Version = "1" }), CancellationToken.None);

            var savedWorkflow = await inMemoryDataContext.Workflows.SingleOrDefaultAsync();

            savedWorkflow.Should().NotBeNull();

            savedWorkflow.ApplicationDataSchema.Should().Be(project.ApplicationDataSchema);
        }
    }
}