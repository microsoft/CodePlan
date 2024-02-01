using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.Files.UploadFile;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SubmitPageOfFilesHandlerTests
{
    public class When_a_branching_condition_is_question_tag : SubmitPageOfFilesTestBase
    {
        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_does_not_pass_if_answer_does_not_match()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "Yes", "No");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None); ;

            response.Value.NextActionId.Should().Be("102");
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_passes_if_answer_matches()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "Yes", "Yes");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None);

            response.Value.NextActionId.Should().Be("101");
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_does_not_pass_if_answer_is_blank_when_must_equal_a_value()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "", "No");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None);

            response.Value.NextActionId.Should().Be("102");
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_does_not_pass_if_answer_is_not_blank_when_must_equal_blank()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "Yes", "");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None);

            response.Value.NextActionId.Should().Be("102");
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_all_pages_are_set_to_active_if_the_condition_passes_and_answer_matches()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "Yes", "Yes");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None);

            var section = GetSection(applicationId, sectionId);
            section.QnAData.Pages.TrueForAll(p => p.Active).Should().BeTrue();
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_does_not_pass_if_answer_is_blank_when_must_equal_a_value_and_contains_is_set()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "", "", "No");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Yes")
            }), CancellationToken.None);

            response.Value.NextActionId.Should().Be("102");
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_the_condition_passes_if_answer_is_one_of_matching_values_when_contains_is_set()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            await SetupQuestionData(applicationId, sectionId, "Possibly,Maybe", "", "Possibly");

            var response = await Handler.Handle(new SubmitPageOfFilesRequest(applicationId, sectionId, "100", new FormFileCollection
            {
                GenerateFile("This is a dummy file", "Q1", "Possibly")
            }), CancellationToken.None);

            response.Value.NextActionId.Should().Be("103");
        }

        private ApplicationSection GetSection(Guid applicationId, Guid sectionId)
        {
            return DataContext.ApplicationSections.FirstOrDefault(sec => sec.ApplicationId == applicationId && sec.Id == sectionId);
        }

        private async Task SetupQuestionData(Guid applicationId, Guid sectionId, string questionValue, string conditionValue, string containsValue = "")
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
                            Questions = new List<Question>{new Question(){QuestionId = "Q1", QuestionTag = "TagName", Input = new Input { Type = "FileUpload" } }},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "101", Conditions = new List<Condition>(){  new Condition{QuestionTag = "TagName", MustEqual = conditionValue } }},
                                new Next(){Action = "NextPage", ReturnId = "103", Conditions = new List<Condition>(){  new Condition{QuestionTag = "TagName", Contains = containsValue }}},
                                new Next(){Action = "NextPage", ReturnId = "102", Conditions = new List<Condition>()}
                            },
                            Active = true
                        },
                        new Page()
                        {
                            PageId = "101",
                            Questions = new List<Question>{new Question(){QuestionId = "Q2", QuestionTag = "TagName2", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "102", Conditions = new List<Condition>()}
                            },
                            Active = false,
                            ActivatedByPageId = "100"
                        },
                        new Page()
                        {
                            PageId = "102",
                            Questions = new List<Question>{new Question(){QuestionId = "Q3", QuestionTag = "TagName3", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>(){
                                new Next(){Action = "NextPage", ReturnId = "103", Conditions = new List<Condition>()}
                            },
                            Active = false,
                            ActivatedByPageId = "100"
                        },
                        new Page()
                        {
                            PageId = "103",
                            Questions = new List<Question>{new Question(){QuestionId = "Q4", QuestionTag = "TagName103", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>(),
                            Active = false,
                            ActivatedByPageId = "100"
                        }
                    }
                }
            });

            var json = "{ \"TagName\" : \"" + questionValue + "\"}";

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = applicationId, ApplicationData = json });

            await DataContext.SaveChangesAsync();
        }
    }
}
