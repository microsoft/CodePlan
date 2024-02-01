using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Commands;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.SetPageAnswersTests
{
    [TestFixture]
    public class SetPageAnswersTestBase
    {
        protected Guid ApplicationId;
        protected Guid SectionId;
        protected SetPageAnswersHandler Handler;
        protected QnaDataContext DataContext;
        protected NotRequiredProcessor NotRequiredProcessor;
        protected TagProcessingService TagProcessingService;
        [SetUp]
        public async Task SetUp()
        {
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            var validator = Substitute.For<IAnswerValidator>();
            NotRequiredProcessor = new NotRequiredProcessor();
            TagProcessingService = new TagProcessingService(DataContext);

            validator.Validate(Arg.Any<List<Answer>>(), Arg.Any<Page>()).Returns(new List<KeyValuePair<string, string>>());

            Handler = new SetPageAnswersHandler(DataContext, validator, NotRequiredProcessor, TagProcessingService);

            ApplicationId = Guid.NewGuid();
            SectionId = Guid.NewGuid();
            await DataContext.ApplicationSections.AddAsync(new ApplicationSection()
            {
                ApplicationId = ApplicationId,
                Id = SectionId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page()
                        {
                            PageId = "1",
                            Questions = new List<Question>{new Question(){QuestionId = "Q1", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "2", Conditions = new List<Condition>(){ new Condition{QuestionId = "Q1", MustEqual = "Yes"}}},
                                new Next(){Action = "NextPage", ReturnId = "4", Conditions = new List<Condition>(){ new Condition{QuestionId = "Q1", MustEqual = "No"}}}
                            },
                            Active = true
                        },
                        new Page()
                        {
                            PageId = "2",
                            Questions = new List<Question>{new Question(){QuestionId = "Q2", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "3", Conditions = new List<Condition>(){  new Condition{QuestionId = "Q2", MustEqual = "Yes"}}},
                                new Next(){Action = "NextPage", ReturnId = "5", Conditions = new List<Condition>(){ new  Condition{QuestionId = "Q2", MustEqual = "No"}}}
                            },
                            Active = false,
                            ActivatedByPageId = "1"
                        },
                        new Page()
                        {
                            PageId = "3",
                            Questions = new List<Question>{new Question(){QuestionId = "Q3", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "7"}
                            },
                            Active = false,
                            ActivatedByPageId = "2"
                        },
                        new Page()
                        {
                            PageId = "4",
                            Questions = new List<Question>{new Question(){QuestionId = "Q4", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "6"}
                            },
                            Active = false,
                            ActivatedByPageId = "1"
                        },
                        new Page()
                        {
                            PageId = "5",
                            Questions = new List<Question>{new Question(){QuestionId = "Q5", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "7"}
                            },
                            Active = false,
                            ActivatedByPageId = "2"
                        },
                        new Page()
                        {
                            PageId = "6",
                            Questions = new List<Question>{new Question(){QuestionId = "Q6", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "8"}
                            },
                            Active = false,
                            ActivatedByPageId = "1"
                        },
                        new Page()
                        {
                            PageId = "7",
                            Questions = new List<Question>{new Question(){QuestionId = "Q7", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "8"}
                            },
                            Active = false,
                            ActivatedByPageId = "1"
                        },
                        new Page()
                        {
                            PageId = "8",
                            Questions = new List<Question>{new Question(){QuestionId = "Q8", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "9"}
                            },
                            Active = true
                        },
                        new Page()
                        {
                            PageId = "9",
                            Questions = new List<Question>{new Question(){QuestionId = "Q9", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "NextPage", ReturnId = "10"}
                            },
                            Active = true
                        },
                        new Page()
                        {
                            PageId = "10",
                            Questions = new List<Question>{new Question(){QuestionId = "Q10", Input = new Input()}},
                            PageOfAnswers = new List<PageOfAnswers>(),
                            Next = new List<Next>
                            {
                                new Next(){Action = "ReturnToSequence", ReturnId = "1"}
                            },
                            Active = true
                        }
                    }
                }
            });

            await DataContext.Applications.AddAsync(new Data.Entities.Application() { Id = ApplicationId, ApplicationData = "{}" });

            await DataContext.SaveChangesAsync();
        }
    }
}