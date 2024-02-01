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
    public class When_GetSection_is_called
    {
        [Test]
        public async Task And_Section_exists_Then_Section_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();

            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<GetSectionRequest>()).Returns(new HandlerResponse<Section>(new Section()
            {
                Id = sectionId,
                SequenceNo = 1,
                SectionNo = 1,
                ApplicationId = applicationId
            }));

            var sectionController = new SectionsController(mediator);

            var result = await sectionController.GetSection(applicationId, sectionId);

            result.Value.Should().BeOfType<Section>();
            result.Value.Id.Should().Be(sectionId);
        }
    }
}