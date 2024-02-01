namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.QnA.Api.Types.Page;
    using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
    using SFA.DAS.QnA.Data.Entities;

    public class When_page_allows_multiple_answers : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId);

            var response = await Handler.Handle(new SetPageAnswersRequest(applicationId, sectionId, "100", new List<Answer>
            {
                new Answer { QuestionId = "Q1", Value = "['1', '2']" }
            }), CancellationToken.None);

            response.Success.Should().BeFalse();
        }

        private async Task SetupQuestionData(Guid applicationId, Guid sectionId)
        {
            await DataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = applicationId,
                Id = sectionId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page()
                        {
                            PageId = "100",
                            AllowMultipleAnswers = true,
                            Questions = new List<Question>
                            {
                                new Question { QuestionId = "Q1", Input = new Input { Type = "TextArea" }}
                            },
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>(),
                            Active = true
                        }
                    }
                }
            }); ;

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = applicationId, ApplicationData = "{}" });

            await DataContext.SaveChangesAsync();
        }
    }
}
