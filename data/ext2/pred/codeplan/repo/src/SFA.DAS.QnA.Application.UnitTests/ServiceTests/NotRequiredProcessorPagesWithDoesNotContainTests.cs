
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;

namespace SFA.DAS.QnA.Application.UnitTests.ServiceTests

{
    [TestFixture]
    public class NotRequiredProcessorPagesWithDoesNotContainTests
    {
        [TestCase("OrgType1", "", true, false, "detail without a toRemove in application data should render as satisfied NRC")]
        [TestCase("", "OrgType2", false, true, "no details should not render as a satisfied NotRequiredCondition")]
        [TestCase("OrgType2", "OrgType1", true, false, "not present so should be removed")]
        [TestCase("OrgType1", "OrgType2", true, false, "not present so should be removed")]
        [TestCase("OrgType1", "OrgType1", false, false, "present so should not be removed")]
        [TestCase("rgType1", "OrgType1", true, false, "not present so should be removed")]
        [TestCase("OrgType", "OrgType1", true, false, "not present so should be removed")]
        [TestCase("orgType1", "OrgType1", false, false, "present so should not be removed")]
        public void When_PagesWithNotRequired_DoesNotContain_conditions_are_removed(string notRequiredConditionValue, string applicationDataValue, bool toRemove, bool singleValue, string explanation)
        {
            var expectedPagesCount = 2;
            if (toRemove)
                expectedPagesCount = 1;

            var pageIdAlwaysPresent = "3";
            var pageIdAbsentIfNotRequired = "2";
            var applicationDataJson = JsonSerializer.Serialize(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JsonNode.Parse(applicationDataJson).AsObject();
            var doesNotContainList = new string[] { notRequiredConditionValue };
            if (!singleValue)
                doesNotContainList = new string[] { "value1", notRequiredConditionValue, "value2" };

            var pages = new List<Page>
            {
                new Page
                {
                    PageId = pageIdAbsentIfNotRequired,
                    NotRequiredConditions = new List<NotRequiredCondition>
                    {
                        new NotRequiredCondition()
                        {
                            Field = "FieldToTest",
                            DoesNotContain = doesNotContainList
                        }
                    },
                    Next = new List<Next>
                    {
                        new Next
                        {
                            Action = "NextPage",
                            ReturnId = "12",
                            Conditions = new List<Condition>()
                        },
                        new Next
                        {
                            Action = "NextPage",
                            ReturnId = "14",
                            Conditions = new List<Condition>()
                        }
                    }
                },
                new Page
                {
                    PageId = pageIdAlwaysPresent,
                    NotRequiredConditions = null
                }
            };

            var notRequiredProcessor = new NotRequiredProcessor();
            var actualPages = notRequiredProcessor.PagesWithoutNotRequired(pages, applicationData);

            Assert.AreEqual(actualPages.ToList().Count, expectedPagesCount);
            Assert.IsTrue(actualPages.Any(p => p.PageId == pageIdAlwaysPresent));
            Assert.AreNotEqual(actualPages.Any(p => p.PageId == pageIdAbsentIfNotRequired), toRemove);
        }
    }

}