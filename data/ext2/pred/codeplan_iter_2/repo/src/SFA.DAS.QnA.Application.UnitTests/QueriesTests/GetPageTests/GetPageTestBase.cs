using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetPageTests
{
    [TestFixture]
    public class GetPageTestBase
    {
        protected GetPageHandler Handler;
        protected Guid ApplicationId;
        protected Guid SectionId;
        protected string PageId;

        [SetUp]
        public async Task SetUp()
        {
            ApplicationId = Guid.NewGuid();
            SectionId = Guid.NewGuid();
            PageId = Guid.NewGuid().ToString();

            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            dataContext.Applications.Add(new Data.Entities.Application()
            {
                Id = ApplicationId,
            });

            dataContext.ApplicationSections.Add(new ApplicationSection()
            {
                Id = SectionId,
                ApplicationId = ApplicationId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page { PageId = PageId }
                    }
                }
            });

            dataContext.SaveChanges();

            Handler = new GetPageHandler(dataContext);
        }
    }
}