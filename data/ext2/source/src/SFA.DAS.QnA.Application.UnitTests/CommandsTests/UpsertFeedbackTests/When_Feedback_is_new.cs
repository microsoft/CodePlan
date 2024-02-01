using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.UpsertFeedbackTests
{
    [TestFixture]
    public class When_Feedback_is_new
    {
        [Test]
        public async Task Then_feedback_entry_is_inserted()
        {
            var createdDateTime = new DateTime(2018, 2, 3);
            SystemTime.UtcNow = () => createdDateTime;

            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();

            var section = new ApplicationSection
            {
                ApplicationId = applicationId,
                Id = sectionId,
                QnAData = new QnAData
                {
                    Pages = new List<Page>
                    {
                        new Page
                        {
                            PageId = "1"
                        }
                    }
                }
            };

            await dataContext.ApplicationSections.AddAsync(section);
            await dataContext.SaveChangesAsync();

            var handler = new UpsertFeedbackHandler(dataContext);

            var newFeedbackId = Guid.NewGuid();
            await handler.Handle(new UpsertFeedbackRequest(applicationId, sectionId, "1",
                new Feedback
                {
                    Date = DateTime.UtcNow,
                    From = "Dave",
                    Id = newFeedbackId,
                    Message = "Feedback message"
                }), CancellationToken.None);

            var updatedSection = await dataContext.ApplicationSections.SingleAsync();

            var updatedPage = updatedSection.QnAData.Pages[0];
            updatedPage.Feedback.Should().NotBeNullOrEmpty();
            updatedPage.Feedback.Count.Should().Be(1);

            var insertedFeedback = updatedPage.Feedback[0];
            insertedFeedback.Date.Should().Be(createdDateTime);
            insertedFeedback.From.Should().Be("Dave");
            insertedFeedback.Id.Should().Be(newFeedbackId);
            insertedFeedback.Message.Should().Be("Feedback message");
            insertedFeedback.IsNew.Should().BeTrue();
            insertedFeedback.IsCompleted.Should().BeFalse();
        }
    }
}