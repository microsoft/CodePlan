using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sections.GetSection;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSectionTests

{
    [TestFixture]
    public class When_section_has_pages_to_be_hidden
    {
        [Test]
        public async Task Then_pages_are_not_returned_in_section()
        {
            var sectionId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            var applicationData = new { OrganisationType = "HEI" };

            dataContext.Applications.Add(new Data.Entities.Application()
            {
                Id = applicationId,
                ApplicationData = JsonSerializer.Serialize(applicationData)
            });

            dataContext.ApplicationSections.Add(new ApplicationSection()
            {
                Id = sectionId,
                ApplicationId = applicationId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>()
                {
                    new Page() {PageId = "1", Active = true},
                    new Page() {PageId = "2", NotRequiredConditions = new List<NotRequiredCondition>(){new NotRequiredCondition(){Field = "OrganisationType", IsOneOf = new []{"HEI"}}}, Active = true},
                    new Page() {PageId = "3", Active = true}
                }
                }
            });

            dataContext.SaveChanges();

            var mapperConfig = new MapperConfiguration(options => { options.CreateMap<ApplicationSection, Section>(); });
            var notRequiredProcessor = new NotRequiredProcessor();

            var handler = new GetSectionHandler(dataContext, mapperConfig.CreateMapper(), notRequiredProcessor);

            var section = await handler.Handle(new GetSectionRequest(applicationId, sectionId), CancellationToken.None);

            section.Value.QnAData.Pages.Count.Should().Be(2);
        }
    }

}