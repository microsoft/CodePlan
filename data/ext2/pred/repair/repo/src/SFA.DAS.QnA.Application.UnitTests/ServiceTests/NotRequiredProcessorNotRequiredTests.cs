using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;

namespace SFA.DAS.QnA.Application.UnitTests.ServiceTests

{
    [TestFixture]
    public class NotRequiredProcessorNotRequiredTests
    {
        [TestCase("OrgType1", "OrgType1", true)]
        [TestCase("OrgType2", "OrgType2", true)]
        [TestCase("OrgType2", "OrgType1", false)]
        [TestCase("OrgType1", "OrgType2", false)]
        [TestCase("OrgType1", "orgType1", false)]
        [TestCase("orgType1", "OrgType1", false)]
        [TestCase("rgType1", "OrgType1", false)]
        [TestCase("OrgType1", "rgType1", false)]

        public void When_NotRequired_conditions_are_expected(string notRequiredConditionValue, string applicationDataValue, bool match)
        {
            var notRequiredProcessor = new NotRequiredProcessor();

            var notRequiredConditions = new List<NotRequiredCondition>
                {new NotRequiredCondition() {Field = "FieldToTest", IsOneOf = new string[] { "value1",notRequiredConditionValue, "value2", "value3"}}};

            var applicationDataJson = JsonSerializer.Serialize(new
            {
                FieldToTest = applicationDataValue
            });

            var applicationData = JsonNode.Parse(applicationDataJson).AsObject();


            var result = notRequiredProcessor.NotRequired(notRequiredConditions, applicationData);

            Assert.AreEqual(match, result);
        }
    }

}