using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Workflows.UpsertWorkflow;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.Workflows.UpsertWorkflowTests
{
    [TestFixture]
    public class When_upsert_called_with_new_workflow
    {
        [Test]
        public async Task Then_Workflow_is_inserted_into_database()
        {
            var inMemoryDataContext = DataContextHelpers.GetInMemoryDataContext();

            var upsertWorkflowHandler = new UpsertWorkflowHandler(inMemoryDataContext);

            var projectId = Guid.NewGuid();
            var workflowId = Guid.NewGuid();
            await upsertWorkflowHandler.Handle(new UpsertWorkflowRequest(projectId, Guid.Empty, new Workflow { Id = workflowId }), CancellationToken.None);

            var createdWorkflow = await inMemoryDataContext.Workflows.SingleOrDefaultAsync(w => w.Id == workflowId);

            createdWorkflow.Should().NotBeNull();
            createdWorkflow.Id.Should().Be(workflowId);
            createdWorkflow.ProjectId.Should().Be(projectId);
        }
    }
}