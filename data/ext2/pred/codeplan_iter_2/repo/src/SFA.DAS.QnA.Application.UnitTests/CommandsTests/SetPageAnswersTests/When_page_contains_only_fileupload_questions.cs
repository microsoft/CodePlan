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

    public class When_page_contains_only_fileupload_questions : SetPageAnswersTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId);

            var response = await Handler.Handle(new SetPageAnswersRequest(applicationId, sectionId, "100", new List<Answer>
            {
                new Answer { QuestionId = "Q1", Value = "File.txt" },
                new Answer { QuestionId = "Q2", Value = "File.txt" },
                new Answer { QuestionId = "Q3", Value = "File.txt" }
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
                            Questions = new List<Question>
                            {
                                new Question { QuestionId = "Q1", Input = new Input { Type = "FileUpload" }},
                                new Question { QuestionId = "Q2", Input = new Input { Type = "FileUpload" }},
                                new Question { QuestionId = "Q3", Input = new Input { Type = "FileUpload" }},
                            },
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>(),
                            Active = true
                        }
                    }
                }
            });

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = applicationId, ApplicationData = "{}" });

            await DataContext.SaveChangesAsync();
        }
    }
}
