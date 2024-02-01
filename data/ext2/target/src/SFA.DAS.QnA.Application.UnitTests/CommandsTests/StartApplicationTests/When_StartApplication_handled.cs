using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.StartApplicationTests
{
    public class When_StartApplication_handled : StartApplicationTestBase
    {
        [Test]
        public async Task Then_a_new_Application_record_is_created()
        {
            await Handler.Handle(new StartApplicationRequest() { UserReference = "dave", WorkflowType = "EPAO" }, CancellationToken.None);

            var newApplications = await DataContext.Applications.ToListAsync();

            newApplications.Count.Should().Be(1);
            newApplications.First().WorkflowId.Should().Be(WorkflowId);
            newApplications.First().ApplicationStatus.Should().Be(ApplicationStatus.InProgress);
            newApplications.First().Reference.Should().Be("dave");
        }

        [Test]
        public async Task Then_the_correct_sequences_are_created()
        {
            await Handler.Handle(new StartApplicationRequest() { UserReference = "dave", WorkflowType = "EPAO" }, CancellationToken.None);

            var newApplication = await DataContext.Applications.FirstAsync();
            var newSequences = await DataContext.ApplicationSequences.OrderBy(s => s.SequenceNo).ToListAsync();

            newSequences.Count.Should().Be(2);
            newSequences.Should().AllBeEquivalentTo(new { ApplicationId = newApplication.Id });
            newSequences[0].SequenceNo.Should().Be(1);
            newSequences[1].SequenceNo.Should().Be(2);
        }

        [Test]
        public async Task Then_the_correct_sections_are_created()
        {
            await Handler.Handle(new StartApplicationRequest() { UserReference = "dave", WorkflowType = "EPAO" }, CancellationToken.None);

            var newApplication = await DataContext.Applications.FirstAsync();
            var newSections = await DataContext.ApplicationSections.OrderBy(s => s.SequenceNo).ThenBy(s => s.SectionNo).ToListAsync();

            newSections.Count.Should().Be(4);
            newSections.Should().AllBeEquivalentTo(new { ApplicationId = newApplication.Id });
            newSections[0].Should().BeEquivalentTo(new { Title = "Section 1", SequenceNo = 1, SectionNo = 1 });
            newSections[1].Should().BeEquivalentTo(new { Title = "Section 2", SequenceNo = 1, SectionNo = 2 });
            newSections[2].Should().BeEquivalentTo(new { Title = "Section 3", SequenceNo = 1, SectionNo = 3 });
            newSections[3].Should().BeEquivalentTo(new { Title = "Section 4", SequenceNo = 2, SectionNo = 4 });
        }

    }
}