using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers.Deserializer;
using SFA.DAS.QnA.Api.Types;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.QnA.Api.UnitTests.HandlerResponseDeserializerTests
{
    public class WhenDeserializingApplicationData
    {
        [Test]
        public void Expected_Properties_And_Values_Are_Returned()
        {
            var handlerResponse = new HandlerResponse<string>() { Value = File.ReadAllText("DeserializerTests/test.json") };

            var sut = HandlerResponseDeserializer.Deserialize(handlerResponse);

            var value = JsonDocument.Parse(sut.ToString()).RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(value.GetProperty("OrganisationReferenceId")
                                 .GetString(),
                                 Is.EqualTo("c3333b62-a07c-415e-8778-84222231b0s1"));

                Assert.That(value.GetProperty("TradingName")
                                 .ValueKind,
                                 Is.EqualTo(JsonValueKind.Null));

                Assert.That(value.GetProperty("UseTradingName")
                                 .GetBoolean(),
                                 Is.False);

                Assert.That(value.GetProperty("company_number")
                                 .GetString(),
                                 Is.Empty);

                Assert.That(value.GetProperty("CompanySummary")
                                 .GetProperty("CompanyNumber")
                                 .GetString(),
                                 Is.EqualTo("RC123456"));

                Assert.That(value.GetProperty("CharitySummary")
                                 .GetProperty("Trustees")
                                 .EnumerateArray()
                                 .First()
                                 .GetProperty("Name")
                                 .GetString(),
                                 Is.EqualTo("test name 1"));
            });
        }
    }
}
