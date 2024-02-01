using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.StartApplication;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ApplicationDataValidatorTests
{
    [TestFixture]
    public class When_valid_applicationData_has_extra_properties
    {
        [Test]
        public void Then_returns_false()
        {
            var validator = new ApplicationDataValidator();

            var schema = @"{   '$schema': 'http://json-schema.org/draft-04/schema#',   'definitions': {},   'id': 'http://example.com/example.json',   'properties': {     'TradingName': {       'anyOf': [             {'type':'string'},             {'type':'null'}         ]     },     'UseTradingName': {       'minLength': 1,       'type': 'boolean'     },     'ContactGivenName': {       'anyOf': [             {'type':'string'},             {'type':'null'}         ]     },     'ReferenceNumber': {        'anyOf': [             {'type':'string'},             {'type':'null'}         ]     },     'StandardCode': {       'anyOf': [             {'type':'string'},             {'type':'null'}         ]     },      'StandardName': {       'anyOf': [             {'type':'string'},             {'type':'null'}         ]     },     'OrganisationReferenceId': {       'minLength': 1,       'type': 'string'     },     'OrganisationName': {       'minLength': 1,       'type': 'string'     }   },   'additionalProperties': false,   'required': [     'OrganisationReferenceId',     'OrganisationName'   ],   'type': 'object'  }";

            var result = validator.IsValid(schema, "{'OrganisationReferenceId':'123', 'OrganisationName':'Org1', 'AnExtraProperty':'abc'}");

            result.Should().BeFalse();
        }
    }
}