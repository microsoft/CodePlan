using System;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.FindNextRequiredActionTests

{
    [TestFixture]
    public class FindNextRequiredActionTestsBase
    {
        protected SetAnswersBase SetAnswersBase;
        protected QnaDataContext QnaDataContext;
        protected Guid ApplicationId;
        protected Next NextAction;
        protected string ApplicationDataJson;
        protected INotRequiredProcessor NotRequiredProcessor;
        protected ITagProcessingService TagProcessingService;
        [SetUp]
        public async Task SetUp()
        {
            QnaDataContext = DataContextHelpers.GetInMemoryDataContext();

            NotRequiredProcessor = new NotRequiredProcessor();
            TagProcessingService = new TagProcessingService(QnaDataContext);
            SetAnswersBase = new SetAnswersBase(QnaDataContext, NotRequiredProcessor, TagProcessingService, null);

            ApplicationId = Guid.NewGuid();

            ApplicationDataJson = JsonSerializer.Serialize(new
            {
                OrgType = "OrgType1"
            });

            await QnaDataContext.Applications.AddAsync(new Data.Entities.Application { Id = ApplicationId, ApplicationData = ApplicationDataJson });

            await QnaDataContext.SaveChangesAsync();

            NextAction = new Next { Action = "NextPage", ReturnId = "2" };
        }
    }

}