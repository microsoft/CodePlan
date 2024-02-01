using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.AddPageAnswer
{
    public class PageHandlerBase
    {
        private readonly QnaDataContext _dataContext;
        protected ApplicationSection Section;
        protected Data.Entities.Application Application;
        protected QnAData QnaData;
        protected Page Page;

        public PageHandlerBase(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task GetSectionAndPage(Guid applicationId, Guid sectionId, string pageId)
        {
            Application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            Section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == sectionId && sec.ApplicationId == applicationId);
            if (Section != null)
            {
                QnaData = new QnAData(Section.QnAData);
                Page = QnaData.Pages.SingleOrDefault(p => p.PageId == pageId);
            }
        }

        protected void MarkFeedbackComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }
    }
}