
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;

namespace SFA.DAS.QnA.Application.UnitTests.ServiceTests

{
    [TestFixture]
    public class NotRequiredProcessorPagesWithoutNotRequiredContainsAllOfTests
    {

        [TestCase(new[] { "value1", "value3" }, "value1,value3,value2", true)]
        [TestCase(new[] { "value3", "value1" }, "value1,value3,value2", true)]
        [TestCase(new[] { "value3", "value1" }, "value3,value2,value1", true)]
        [TestCase(new[] { "value1" }, "value1,value3,value2", true)]
        [TestCase(new[] { "value2" }, "value1,value3,value2", true)]
        [TestCase(new[] { "value2", "value2" }, "value1,value3,value2", true)]
        [TestCase(new[] { "value2", "value4" }, "value1,value3,value2", false)]
        [TestCase(new[] { "value4", "value2" }, "value1,value3,value2", false)]
        [TestCase(new[] { "value" }, "value1,value3,value2", false)]
        [TestCase(new string[] { }, "value1,value3,value2", false)]
        [TestCase(null, "value1,value3,value2", false)]
        [TestCase(new[] { "value" }, "", false)]
        [TestCase(new[] { "value" }, null, false)]
        [TestCase(new string[] { }, null, false)]
        [TestCase(null, null, false)]
        public void When_PagesWithNotRequired_conditions_with_containsAllOf(string[] containsAllValues, string applicationDataValue, bool match)
        {
            var expectedPagesCount = 1;
            if (!match)
                expectedPagesCount = 2;

            var pageIdAlwaysPresent = "3";
            var pageIdAbsentIfNotRequired = "2";
            var applicationDataJson = JsonSerializer.Serialize(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JsonNode.Parse(applicationDataJson).AsObject();

            var pages = new List<Page>
            {
                new Page
                {
                    PageId = pageIdAbsentIfNotRequired,
                    NotRequiredConditions = new List<NotRequiredCondition>
                    {
                        new NotRequiredCondition
                        {
                            Field = "FieldToTest",
                            ContainsAllOf = containsAllValues
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

            Assert.AreEqual(actualPages.Count(), expectedPagesCount);
            Assert.IsTrue(actualPages.Any(p => p.PageId == pageIdAlwaysPresent));
            Assert.AreNotEqual(actualPages.Any(p => p.PageId == pageIdAbsentIfNotRequired), match);
        }
    }

}