using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.GetWorkflows;

namespace SFA.DAS.QnA.Api.UnitTests.WorkflowControllerTests
{
    public class When_GetWorkflows_is_called
    {
        [Test]
        public async Task Should_return_a_list_of_workflowresults()
        {
            var mediator = Substitute.For<IMediator>();

            var workflowId = Guid.NewGuid();

            var workflowResponse = new Workflow()
            {
                Description = "Workflow 1",
                Id = workflowId,
                Type = "EPAO",
                Version = "1"
            };
            mediator.Send(Arg.Any<GetWorkflowsRequest>(), Arg.Any<CancellationToken>())
                .Returns(new HandlerResponse<List<Workflow>>(new List<Workflow>
                {
                    workflowResponse
                }));

            var workflowController = new WorkflowsController(mediator);

            var result = await workflowController.GetWorkflows(Guid.NewGuid());

            result.Value.Should().BeOfType<List<Workflow>>();
            result.Value.Count.Should().Be(1);
            result.Value.First().Should().BeEquivalentTo(workflowResponse);
        }
    }
}