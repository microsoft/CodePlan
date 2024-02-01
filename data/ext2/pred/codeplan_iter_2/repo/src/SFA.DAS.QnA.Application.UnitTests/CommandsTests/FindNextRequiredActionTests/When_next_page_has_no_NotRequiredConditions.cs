using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.FindNextRequiredActionTests

{
    public class When_next_page_has_no_NotRequiredConditions : FindNextRequiredActionTestsBase
    {
        [Test]
        public async Task For_null_NotRequiredConditions_then_the_same_nextAction_is_returned()
        {
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData
                {
                    Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = null
                    }
                }
                }
            };

            var applicationData = JsonNode.Parse(ApplicationDataJson);
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, NextAction, applicationData);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(NextAction);
        }

        [Test]
        public async Task For_empty_NotRequiredConditions_then_the_same_nextAction_is_returned()
        {
            var section = new ApplicationSection
            {
                ApplicationId = ApplicationId,
                QnAData = new QnAData
                {
                    Pages = new List<Page>
                {
                    new Page
                    {
                        PageId = "2",
                        NotRequiredConditions = new List<NotRequiredCondition>()
                    }
                }
                }
            };

            var applicationData = JsonNode.Parse(ApplicationDataJson);
            var nextActionAfterFindingNextAction = SetAnswersBase.FindNextRequiredAction(section, NextAction, applicationData);
            nextActionAfterFindingNextAction.Should().BeEquivalentTo(NextAction);
        }
    }

}