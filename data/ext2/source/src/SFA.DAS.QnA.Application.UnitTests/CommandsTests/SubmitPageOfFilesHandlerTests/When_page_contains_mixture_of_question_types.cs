namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    using SFA.DAS.QnA.Api.Types.Page;
    using SFA.DAS.QnA.Application.Commands.Files.UploadFile;
    using SFA.DAS.QnA.Data.Entities;

    public class When_page_contains_mixture_of_question_types : SubmitPageOfFilesTestBase
    {
        [Test]
        public async Task Then_validation_error_occurs()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId);

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "1", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "File.txt")
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
                            PageId = "1",
                            Questions = new List<Question>
                            {
                                new Question { QuestionId = "Q1", Input = new Input { Type = "FileUpload" }},
                                new Question { QuestionId = "Q2", Input = new Input { Type = "TextArea" }}
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
