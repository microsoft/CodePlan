using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.UpsertFeedbackTests
{
    [TestFixture]
    public class When_Feedback_exists
    {
        private UpsertFeedbackHandler _handler;
        private QnaDataContext _dataContext;
        private Guid _applicationId;
        private Guid _sectionId;
        private Guid _feedbackId;

        [SetUp]
        public async Task SetUp()
        {
            _dataContext = DataContextHelpers.GetInMemoryDataContext();

            _applicationId = Guid.NewGuid();
            _sectionId = Guid.NewGuid();

            _feedbackId = Guid.NewGuid();
            var section = new ApplicationSection
            {
                ApplicationId = _applicationId,
                Id = _sectionId,
                QnAData = new QnAData
                {
                    Pages = new List<Page>
                    {
                        new Page
                        {
                            PageId = "1",
                            Feedback = new List<Feedback>
                            {
                                new Feedback
                                {
                                    Id = _feedbackId
                                }
                            }
                        }
                    }
                }
            };

            await _dataContext.ApplicationSections.AddAsync(section);
            await _dataContext.SaveChangesAsync();

            _handler = new UpsertFeedbackHandler(_dataContext);
        }

        [Test]
        public async Task Then_feedback_entry_is_updated()
        {
            await _handler.Handle(new UpsertFeedbackRequest(_applicationId, _sectionId, "1",
                new Feedback
                {
                    Date = DateTime.UtcNow,
                    From = "Dave",
                    Id = _feedbackId,
                    Message = "Feedback message"
                }), CancellationToken.None);

            var updatedSection = await _dataContext.ApplicationSections.SingleAsync();
            var updatedPage = updatedSection.QnAData.Pages[0];
            var updatedFeedback = updatedPage.Feedback[0];

            updatedFeedback.Id.Should().Be(_feedbackId);
            updatedFeedback.From.Should().Be("Dave");
            updatedFeedback.Message.Should().Be("Feedback message");
        }

        [Test]
        public async Task Then_feedback_entry_is_not_inserted()
        {
            var existingSection = await _dataContext.ApplicationSections.SingleAsync();

            var existingPage = existingSection.QnAData.Pages[0];
            existingPage.Feedback.Should().NotBeNullOrEmpty();
            existingPage.Feedback.Count.Should().Be(1);

            await _handler.Handle(new UpsertFeedbackRequest(_applicationId, _sectionId, "1",
                new Feedback
                {
                    Date = DateTime.UtcNow,
                    From = "Dave",
                    Id = _feedbackId,
                    Message = "Feedback message"
                }), CancellationToken.None);

            var updatedSection = await _dataContext.ApplicationSections.SingleAsync();

            var updatedPage = updatedSection.QnAData.Pages[0];
            updatedPage.Feedback.Should().NotBeNullOrEmpty();
            updatedPage.Feedback.Count.Should().Be(1);

            var updatedFeedback = updatedPage.Feedback[0];
            updatedFeedback.Id.Should().Be(_feedbackId);
        }
    }
}