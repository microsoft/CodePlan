
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Services

{
    public class TagProcessingService : ITagProcessingService
    {
        private readonly QnaDataContext _dataContext;

        public TagProcessingService(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void ClearDeactivatedTags(Guid applicationId, Guid sectionId)
        {
            var section = _dataContext.ApplicationSections.SingleOrDefault(sec =>
                sec.Id == sectionId && sec.ApplicationId == applicationId);
            if (section == null) return;
            var pages = new QnAData(section.QnAData).Pages;
            var pagesActive = pages.Where(p => p.Active);
            var pagesInactive = pages.Where(p => !p.Active);

            var deactivatedTags = (from page in pagesInactive from question in page.Questions.Where(p => !string.IsNullOrEmpty(p.QuestionTag)) select question.QuestionTag).Distinct().ToList();
            var activeTags = (from page in pagesActive from question in page.Questions.Where(p => !string.IsNullOrEmpty(p.QuestionTag)) select question.QuestionTag).Distinct().ToList();

            // remove any activetags within deactivatedTags as some branches use tagnames twice for distinct branches
            foreach (var tag in activeTags.Where(tag => deactivatedTags.Contains(tag)))
            {
                deactivatedTags.Remove(tag);
            }

            if (deactivatedTags.Count < 1)
                return;

            var application = _dataContext.Applications.SingleOrDefault(app => app.Id == applicationId);
            if (application == null) return;
            var applicationData = JsonNode.Parse(application.ApplicationData ?? "{}");

            foreach (var tag in deactivatedTags)
            {
                applicationData.AsObject().Remove(tag);
            }

            application.ApplicationData = applicationData.ToString();
            _dataContext.SaveChanges();
        }
    }

}