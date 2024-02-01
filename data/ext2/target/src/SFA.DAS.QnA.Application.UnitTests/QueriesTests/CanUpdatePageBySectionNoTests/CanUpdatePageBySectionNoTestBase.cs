using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.CanUpdatePageBySectionNoTests
{
    [TestFixture]
    public class CanUpdatePageBySectionNoTestBase
    {
        protected CanUpdatePageBySectionNoHandler Handler;
        protected Guid ApplicationId;
        protected int SequenceNo;
        protected int SectionNo;
        protected string ActivePageId;
        protected string InactivePageId;

        [SetUp]
        public async Task SetUp()
        {
            ApplicationId = Guid.NewGuid();
            SequenceNo = 1;
            SectionNo = 1;
            ActivePageId = Guid.NewGuid().ToString();
            InactivePageId = Guid.NewGuid().ToString();

            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            dataContext.Applications.Add(new Data.Entities.Application()
            {
                Id = ApplicationId,
            });

            dataContext.ApplicationSections.Add(new ApplicationSection()
            {
                SectionNo = SectionNo,
                SequenceNo = SequenceNo,
                ApplicationId = ApplicationId,
                QnAData = new QnAData()
                {
                    Pages = new List<Page>
                    {
                        new Page { PageId = ActivePageId, Active = true },
                        new Page { PageId = InactivePageId, Active = false },
                    }
                }
            });

            dataContext.SaveChanges();

            Handler = new CanUpdatePageBySectionNoHandler(dataContext);
        }
    }
}