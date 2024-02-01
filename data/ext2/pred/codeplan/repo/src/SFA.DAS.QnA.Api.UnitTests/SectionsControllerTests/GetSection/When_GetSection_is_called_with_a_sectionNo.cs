using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sections.GetSection;

namespace SFA.DAS.QnA.Api.UnitTests.SectionsControllerTests.GetSection
{
    [TestFixture]
    public class When_GetSection_is_called_with_a_sectionNo
    {
        [Test]
        public async Task And_Section_exists_Then_Section_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var sequenceNo = 1;
            var sectionNo = 1;

            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<GetSectionBySectionNoRequest>()).Returns(new HandlerResponse<Section>(new Section()
            {
                Id = Guid.NewGuid(),
                SequenceNo = sequenceNo,
                SectionNo = sectionNo,
                ApplicationId = applicationId
            }));

            var sectionController = new SectionsController(mediator);

            var result = await sectionController.GetSectionBySectionNo(applicationId, sequenceNo, sectionNo);

            result.Value.Should().BeOfType<Section>();
            result.Value.SequenceNo.Should().Be(sequenceNo);
            result.Value.SectionNo.Should().Be(sectionNo);
        }
    }
}