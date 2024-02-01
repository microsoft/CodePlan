using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.ResetPagesToIncomplete;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.Handlers
{
    [TestFixture]
    public class ResetPagesToIncompleteHandlerTests
    {
        private ResetPagesToIncompleteHandler _handler;
        protected QnaDataContext DataContext;
        private Guid _applicationId;
        private const int SequenceNo = 1;
        private const int SectionNo = 5;

        [SetUp]
        public void TestSetup()
        {
            _applicationId = Guid.NewGuid();
            DataContext = DataContextHelpers.GetInMemoryDataContext();
            _handler = new ResetPagesToIncompleteHandler(DataContext,
                                    Mock.Of<INotRequiredProcessor>(),
                                    Mock.Of<ITagProcessingService>(),
                                    Mock.Of<ILogger<ResetPagesToIncompleteHandler>>());
        }

        [Test]
        public async Task Handle_OnSuccessfulUpdate_ResetsPagesToIncomplete()
        {
            var pages = new List<Page>
            {
                new Page { PageId = "Page1", Complete = true },
                new Page { PageId = "Page2", Complete = true },
                new Page { PageId = "Page3", Complete=true },
                new Page { PageId = "Page4", Complete = true},
                new Page { PageId = "Page5", Complete = true}
            };
            var pagesToExclude = new List<string>
            {
                "Page1",
                "Page3",
                "Page5"
            };

            var applicationSection = new ApplicationSection
            {
                ApplicationId = _applicationId,
                SequenceNo = SequenceNo,
                SectionNo = SectionNo,
                QnAData = new QnAData { Pages = pages }
            };

            await DataContext.ApplicationSections.AddAsync(applicationSection);
            await DataContext.SaveChangesAsync();
            var request = new ResetPagesToIncompleteRequest(_applicationId, SequenceNo, SectionNo, pagesToExclude);

            var result = await _handler.Handle(request, new CancellationToken());

            var updatedPages = DataContext.ApplicationSections.First().QnAData.Pages;

            Assert.AreEqual(5, updatedPages.Count);

            foreach (var page in updatedPages)
            {
                if (pagesToExclude.Contains(page.PageId))
                {
                    Assert.IsTrue(page.Complete);
                }
                else
                {
                    Assert.IsFalse(page.Complete);
                }
            }

            Assert.IsTrue(result.Value);
        }

        [Test]
        public async Task Handle_NoMatchingPageIds_ResetsAllPagesToIncomplete()
        {
            var pages = new List<Page>
            {
                new Page { PageId = "Page1", Complete = true },
                new Page { PageId = "Page2", Complete = true },
                new Page { PageId = "Page3", Complete=true },
                new Page { PageId = "Page4", Complete = true},
                new Page { PageId = "Page5", Complete = true}
            };
            var pagesToExclude = new List<string>
            {
                "Page7",
                "Page8",
                "Page9"
            };

            var applicationSection = new ApplicationSection
            {
                ApplicationId = _applicationId,
                SequenceNo = SequenceNo,
                SectionNo = SectionNo,
                QnAData = new QnAData { Pages = pages }
            };

            await DataContext.ApplicationSections.AddAsync(applicationSection);
            await DataContext.SaveChangesAsync();
            var request = new ResetPagesToIncompleteRequest(_applicationId, SequenceNo, SectionNo, pagesToExclude);

            var result = await _handler.Handle(request, new CancellationToken());

            var updatedPages = DataContext.ApplicationSections.First().QnAData.Pages;

            Assert.AreEqual(5, updatedPages.Count);

            foreach (var page in updatedPages)
            {
                Assert.IsFalse(page.Complete);
            }

            Assert.IsTrue(result.Value);
        }

        [Test]
        public async Task Handle_AllMatchingPageIds_ResetsNoPagesToIncomplete()
        {
            var pages = new List<Page>
            {
                new Page { PageId = "Page1", Complete = true },
                new Page { PageId = "Page2", Complete = true },
                new Page { PageId = "Page3", Complete=true },
                new Page { PageId = "Page4", Complete = true},
                new Page { PageId = "Page5", Complete = true}
            };
            var pagesToExclude = new List<string>
            {
                "Page1",
                "Page2",
                "Page3",
                "Page4",
                "Page5"
            };

            var applicationSection = new ApplicationSection
            {
                ApplicationId = _applicationId,
                SequenceNo = SequenceNo,
                SectionNo = SectionNo,
                QnAData = new QnAData { Pages = pages }
            };

            await DataContext.ApplicationSections.AddAsync(applicationSection);
            await DataContext.SaveChangesAsync();
            var request = new ResetPagesToIncompleteRequest(_applicationId, SequenceNo, SectionNo, pagesToExclude);

            var result = await _handler.Handle(request, new CancellationToken());

            var updatedPages = DataContext.ApplicationSections.First().QnAData.Pages;

            Assert.AreEqual(5, updatedPages.Count);

            foreach (var page in updatedPages)
            {
                Assert.IsTrue(page.Complete);
            }

            Assert.IsTrue(result.Value);
        }

        [Test]
        public async Task Handle_NoPagesInSection_RequestSucceeds()
        {
            var pagesToExclude = new List<string>
            {
                "Page1",
                "Page2",
                "Page3"
            };

            var applicationSection = new ApplicationSection
            {
                ApplicationId = _applicationId,
                SequenceNo = SequenceNo,
                SectionNo = SectionNo,
                QnAData = new QnAData { Pages = null }
            };

            await DataContext.ApplicationSections.AddAsync(applicationSection);
            await DataContext.SaveChangesAsync();
            var request = new ResetPagesToIncompleteRequest(_applicationId, SequenceNo, SectionNo, pagesToExclude);

            var result = await _handler.Handle(request, new CancellationToken());

            var qnaData = DataContext.ApplicationSections.First().QnAData;
            Assert.IsNull(qnaData.Pages);
            Assert.IsTrue(result.Value);
        }

        [Test]
        public async Task Handle_NoMatchingSection_RequestSucceeds()
        {
            var alternativeSectionNo = 6;

            var pagesToExclude = new List<string>
            {
                "Page1",
                "Page2",
                "Page3"
            };

            var applicationSection = new ApplicationSection
            {
                ApplicationId = _applicationId,
                SequenceNo = SequenceNo,
                SectionNo = alternativeSectionNo,
                QnAData = new QnAData { Pages = null }
            };

            await DataContext.ApplicationSections.AddAsync(applicationSection);
            await DataContext.SaveChangesAsync();
            var request = new ResetPagesToIncompleteRequest(_applicationId, SequenceNo, SectionNo, pagesToExclude);

            var result = await _handler.Handle(request, new CancellationToken());
            var sectionPresent = DataContext.ApplicationSections.Any(x => x.ApplicationId == _applicationId && x.SequenceNo == SequenceNo && x.SectionNo == SectionNo);
            Assert.IsFalse(sectionPresent);
            Assert.IsTrue(result.Value);
        }
    }
}

