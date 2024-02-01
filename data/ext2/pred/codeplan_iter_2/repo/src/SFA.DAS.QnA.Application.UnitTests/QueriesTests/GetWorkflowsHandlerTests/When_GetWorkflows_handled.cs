using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetWorkflowsHandlerTests
{
    [TestFixture]
    public class When_GetWorkflows_handled
    {
        [Test]
        public async Task Then_only_live_workflows_are_returned()
        {
            var dbContextOptions = new DbContextOptionsBuilder<QnaDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new QnaDataContext(dbContextOptions);

            var projectId = Guid.NewGuid();

            context.Workflows.AddRange(new[]
            {
                new Workflow(){Id = Guid.NewGuid(), Status = WorkflowStatus.Live, ProjectId = projectId},
                new Workflow(){Id = Guid.NewGuid(), ProjectId = projectId},
                new Workflow(){Id = Guid.NewGuid(), Status = WorkflowStatus.Live, ProjectId = projectId},
            });

            await context.SaveChangesAsync();

            var mapper = new Mapper(new MapperConfiguration(config => { config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); }));

            var handler = new GetWorkflowsHandler(context, mapper);

            var results = await handler.Handle(new GetWorkflowsRequest(projectId), CancellationToken.None);

            results.Value.Should().BeOfType<List<Workflow>>();
            results.Value.Count.Should().Be(2);
        }
    }
}