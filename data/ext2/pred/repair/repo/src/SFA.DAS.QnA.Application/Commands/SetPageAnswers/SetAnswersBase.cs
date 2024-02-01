using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers

{
    public class SetAnswersBase
    {
        protected readonly QnaDataContext _dataContext;
        protected readonly INotRequiredProcessor _notRequiredProcessor;
        protected readonly ITagProcessingService _tagProcessingService;
        protected readonly IAnswerValidator _answerValidator;

        public SetAnswersBase(QnaDataContext dataContext, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService, IAnswerValidator answerValidator)
        {
            _dataContext = dataContext;
            _notRequiredProcessor = notRequiredProcessor;
            _tagProcessingService = tagProcessingService;
            _answerValidator = answerValidator;
        }

        protected void SaveAnswersIntoPage(ApplicationSection section, string pageId, List<Answer> submittedAnswers)
        {
            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    var answers = GetAnswersFromRequest(submittedAnswers);
                    page.PageOfAnswers = new List<PageOfAnswers>(new[] { new PageOfAnswers() { Answers = answers } });

                    MarkPageAsComplete(page);
                    MarkPageFeedbackAsComplete(page);

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                }
            }
        }

        protected void UpdateApplicationData(string pageId, List<Answer> submittedAnswers, ApplicationSection section, Data.Entities.Application application)
        {
            if (application != null)
            {
                var applicationData = JObject.Parse(application.ApplicationData ?? "{}");

                var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    var questionTagsWhichHaveBeenUpdated = new List<string>();
                    var answers = GetAnswersFromRequest(submittedAnswers);

                    foreach (var question in page.Questions)
                    {
                        SetApplicationDataField(question, answers, applicationData);
                        if (!string.IsNullOrWhiteSpace(question.QuestionTag))
                            questionTagsWhichHaveBeenUpdated.Add(question.QuestionTag);

                        if (question.Input.Options != null)
                        {
                            foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                            {
                                foreach (var furtherQuestion in option.FurtherQuestions)
                                {
                                    SetApplicationDataField(furtherQuestion, answers, applicationData);
                                    if (!string.IsNullOrWhiteSpace(furtherQuestion.QuestionTag))
                                        questionTagsWhichHaveBeenUpdated.Add(furtherQuestion.QuestionTag);
                                }
                            }
                        }
                    }

                    application.ApplicationData = applicationData.ToString(Formatting.None);

                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application, questionTagsWhichHaveBeenUpdated);
                    _tagProcessingService.ClearDeactivatedTags(application.Id, section.Id);

                }
            }
        }

        protected HandlerResponse<SetPageAnswersResponse> ValidateSetPageAnswersRequest(string pageId, List<Answer> submittedAnswers, ApplicationSection section)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if (submittedAnswers is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "No answers specified.");
            }
            else if (submittedAnswers.Any(a => a.QuestionId is null))
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "All answers must specify which question they are related to.");
            }
            else if (page.AllowMultipleAnswers)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");
            }
            else if (page.Questions.Any())
            {
                var answers = GetAnswersFromRequest(submittedAnswers);

                if (page.Questions.All(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for FileUpload questions. Use Upload / DeleteFile instead.");
                }
                else if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }
                else if (page.Questions.Count > answers.Count)
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"Number of Answers supplied ({answers.Count}) does not match number of first level Questions on page ({page.Questions.Count}).");
                }

                var validationErrors = _answerValidator.Validate(answers, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }
            }

            return null;
        }

        protected HandlerResponse<ResetPageAnswersResponse> ValidateResetPageAnswersRequest(string pageId, ApplicationSection section)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if (page.Questions.Any())
            {
                if (page.Questions.All(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for FileUpload questions. Use Upload / DeleteFile instead.");
                }
                else if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<ResetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }
            }

            return null;
        }

        protected HandlerResponse<ResetSectionAnswersResponse> ValidateSectionAnswersRequest(ApplicationSection section)
        {
            if (section is null)
            {
                return new HandlerResponse<ResetSectionAnswersResponse>(success: false, message: "The section does not exist.");
            }

            return null;
        }

        protected static void SetApplicationDataField(Question question, List<Answer> answers, JsonNode applicationData)
        {
            if (question != null && applicationData != null)
            {
                var questionTag = question.QuestionTag;
                var questionTagAnswer = answers?.SingleOrDefault(a => a.QuestionId == question.QuestionId)?.Value;

                if (!string.IsNullOrWhiteSpace(questionTag))
                {
                    if (applicationData.AsObject().ContainsKey(questionTag))
                    {
                        applicationData.AsObject()[questionTag] = questionTagAnswer;
                    }
                    else
                    {
                        applicationData.AsObject().Add(questionTag, questionTagAnswer);
                    }
                }
            }
        }

        private static List<Answer> GetAnswersFromRequest(List<Answer> submittedAnswers)
        {
            var answers = new List<Answer>();

            if (submittedAnswers != null)
            {
                answers.AddRange(submittedAnswers);
            }

            return answers;
        }

        protected List<Next> GetCheckboxListMatchingNextActionsForPage(ApplicationSection section, Data.Entities.Application application, string pageId)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return new List<Next>();
            }
            else if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }
            else if (page.Questions.All(q => !"CheckboxList".Equals(q.Input.Type, StringComparison.InvariantCultureIgnoreCase) && !"ComplexCheckboxList".Equals(q.Input.Type, StringComparison.InvariantCultureIgnoreCase)))
            {
                return new List<Next>();
            }

            var matchingNexts = new List<Next>();

            foreach (var next in page.Next)
            {
                var allConditionsSatisfied = true;

                if (next.Conditions != null && next.Conditions.Any())
                {
                    foreach (var condition in next.Conditions.Where(c => c.Contains != null))
                    {
                        var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                        var answers = page.PageOfAnswers?.FirstOrDefault()?.Answers;
                        var answer = answers?.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                        if ("CheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase) || ("ComplexCheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            if (answer == null)
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                            else
                            {
                                var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

                                if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                        }
                    }
                }

                if (allConditionsSatisfied)
                {
                    // NOTE: In this version we add all of the matching conditions.
                    matchingNexts.Add(next);
                }
            }

            var applicationData = JObject.Parse(application?.ApplicationData ?? "{}");
            var matchingNextsToReturn = new List<Next>();

            foreach (var matchingNext in matchingNexts)
            {
                var nextAction = FindNextRequiredAction(section, matchingNext, applicationData);

                if (nextAction != null)
                {
                    matchingNextsToReturn.Add(nextAction);
                }
            }

            return matchingNextsToReturn;
        }

        protected Next GetNextActionForPage(ApplicationSection section, Data.Entities.Application application, string pageId)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return null;
            }
            else if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }

            var applicationData = JObject.Parse(application?.ApplicationData ?? "{}");

            Next nextAction = null;

            if (page.Next.Count == 1)
            {
                nextAction = page.Next.Single();
            }

            foreach (var next in page.Next)
            {
                var allConditionsSatisfied = true;

                if (next.Conditions != null && next.Conditions.Any())
                {
                    foreach (var condition in next.Conditions)
                    {
                        if (!String.IsNullOrWhiteSpace(condition.QuestionTag))
                        {
                            var questionTagValue = applicationData[condition.QuestionTag];
                            var questionTag = questionTagValue?.Value<string>();
                            allConditionsSatisfied = CheckAllConditionsSatisfied(condition, questionTag);

                            if (!allConditionsSatisfied)
                                break;
                        }
                        else
                        {
                            var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                            var answers = page.PageOfAnswers?.FirstOrDefault()?.Answers;
                            var answer = answers?.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                            if ("CheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase) || ("ComplexCheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (answer == null)
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                                else
                                {
                                    var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

                                    if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                    {
                                        allConditionsSatisfied = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (allConditionsSatisfied)
                {
                    // NOTE: In this version we return the first Next action that is satisfied. In the Checkbox version we return all of the matching ones.
                    // At some point we should check this intended behaviour works if multiple Next actions are satisfied BUT one of them are actually not required.
                    nextAction = next;
                    break;
                }
            }

            return FindNextRequiredAction(section, nextAction, applicationData);
        }

        private static bool CheckAllConditionsSatisfied(Condition condition, string questionTag)
        {
            bool allConditionsSatisified = true;

            if ((string.IsNullOrEmpty(condition.Contains)) && ((!string.IsNullOrEmpty(condition.MustEqual) && questionTag != condition.MustEqual)
                || (string.IsNullOrEmpty(condition.MustEqual) && !string.IsNullOrEmpty(questionTag))))
            {
                allConditionsSatisified = false;
            }

            if (!string.IsNullOrEmpty(condition.Contains))
            {
                var listOfAnswers = string.IsNullOrWhiteSpace(questionTag) ? new string[] { } : questionTag.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (!listOfAnswers.Contains(condition.Contains))
                {
                    allConditionsSatisified = false;
                }
            }

            return allConditionsSatisified;
        }


        public Next FindNextRequiredAction(ApplicationSection section, Next nextAction, JObject applicationData)
        {
            if (section?.QnAData is null || nextAction is null || nextAction.Action != "NextPage") return nextAction;

            var isRequiredNextAction = true;

            // Check here for any NotRequiredConditions on the next page.
            var nextPage = section.QnAData?.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);

            if (nextPage is null || applicationData is null)
            {
                return nextAction;
            }
            else if (nextPage.NotRequiredConditions != null && nextPage.NotRequiredConditions.Any())
            {
                nextPage.NotRequired = _notRequiredProcessor.NotRequired(nextPage.NotRequiredConditions, applicationData);
                if (nextPage.NotRequired)
                {
                    isRequiredNextAction = false;
                }
            }

            if (isRequiredNextAction || nextPage.Next is null) return nextAction;

            // Get the next default action from this page.
            if (nextPage.Next.Count == 1)
            {
                nextAction = nextPage.Next.Single();
            }
            else if (nextPage.Next.Any(n => n.Conditions == null))
            {
                // For some reason null Conditions takes precedence over empty Conditions
                nextAction = nextPage.Next.Last(n => n.Conditions == null);
            }
            else if (nextPage.Next.Any(n => n.Conditions.Count == 0))
            {
                nextAction = nextPage.Next.Last(n => n.Conditions.Count == 0);
            }
            else
            {
                nextAction = nextPage.Next.Last();
            }

            // NOTE the recursion!
            return FindNextRequiredAction(section, nextAction, applicationData);
        }

        protected void MarkPageAsComplete(Page page)
        {
            page.Complete = true;
        }

        protected async Task ResetPageAnswers(string pageId, Guid applicationId, ApplicationSection section, CancellationToken cancellationToken, bool completeFeedback = true, bool removeFeedback = false)
        {
            if (section != null)
            {
                var page = section.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    page.PageOfAnswers = new List<PageOfAnswers>();
                    page.Complete = false;

                    if (removeFeedback)
                    {
                        RemovePageFeedback(page);
                    }
                    else if (completeFeedback)
                    {
                        MarkPageFeedbackAsComplete(page);
                    }

                    // Assign QnAData using a new object so Entity Framework will pick up changes
                    section.QnAData = new QnAData(section.QnAData);
                }

                var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId, cancellationToken);
                UpdateApplicationData(pageId, new List<Answer>(), section, application);

                var nextAction = GetNextActionForPage(section, application, pageId);
                var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, pageId);

                SetStatusOfNextPagesBasedOnDeemedNextActions(section, pageId, nextAction, checkboxListAllNexts);
            }
        }

        protected void MarkPageFeedbackAsComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }

        protected void RemovePageFeedback(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback = new List<Feedback>();
            }
        }

        protected void SetStatusOfNextPagesBasedOnDeemedNextActions(ApplicationSection section, string pageId, Next deemedNextAction, List<Next> deemedCheckboxListNextActions)
        {
            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    if (deemedCheckboxListNextActions != null && deemedCheckboxListNextActions.Any())
                    {
                        foreach (var checkboxNextAction in deemedCheckboxListNextActions)
                        {
                            DeactivateDependentPages(checkboxNextAction, page.PageId, qnaData, page);
                        }

                        foreach (var checkboxNextAction in deemedCheckboxListNextActions)
                        {
                            ActivateDependentPages(checkboxNextAction, page.PageId, qnaData, page);
                        }
                    }
                    else if (deemedNextAction != null)
                    {
                        DeactivateDependentPages(deemedNextAction, page.PageId, qnaData, page);
                        ActivateDependentPages(deemedNextAction, page.PageId, qnaData, page);
                    }
                    else if (deemedNextAction is null && page.Next != null)
                    {
                        // NOTE: This should only occur when resetting page answers and all of the next actions are based on particular answer being provided
                        // Unforunately we don't have one of those answers so we must deactivate all dependant pages
                        foreach (var nextAction in page.Next)
                        {
                            DeactivateDependentPages(nextAction, page.PageId, qnaData, page);
                        }
                    }

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                }
            }
        }

        protected void SetStatusOfAllPagesBasedOnUpdatedQuestionTags(Data.Entities.Application application, List<string> questionTags)
        {
            if (questionTags != null && questionTags.Count > 0)
            {
                var sections = _dataContext.ApplicationSections.Where(sec => sec.ApplicationId == application.Id);

                // Go through each section in the application
                foreach (var section in sections)
                {
                    // Get the list of pages that contain one of QuestionTags in the next condition
                    var pagesToProcess = new List<Page>();
                    foreach (var questionTag in questionTags.Distinct())
                    {
                        var questionTagPages = section.QnAData.Pages.Where(p => !p.AllowMultipleAnswers && p.Next.SelectMany(n => n.Conditions).Select(c => c.QuestionTag).Contains(questionTag));
                        pagesToProcess.AddRange(questionTagPages);
                    }

                    if (pagesToProcess.Any())
                    {
                        // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                        var qnaData = new QnAData(section.QnAData);

                        // Deactivate & Activate affected pages accordingly
                        foreach (var page in pagesToProcess)
                        {
                            if (page.PageOfAnswers != null && page.PageOfAnswers.Count > 0)
                            {
                                var nextAction = GetNextActionForPage(section, application, page.PageId);
                                if (nextAction?.Conditions != null)
                                {
                                    DeactivateDependentPages(nextAction, page.PageId, qnaData, page);
                                    ActivateDependentPages(nextAction, page.PageId, qnaData, page);
                                }
                            }
                        }

                        // Assign QnAData back so Entity Framework will pick up changes
                        section.QnAData = qnaData;
                    }
                }
            }
        }

        protected void DeactivateDependentPages(Next chosenAction, string branchingPageId, QnAData qnaData, Page page, bool subPages = false)
        {
            if (chosenAction != null && page != null)
            {
                // process all sub pages or those which are not the chosen action
                foreach (var nextAction in page.Next.Where(n => subPages || !(n.Action == chosenAction.Action && n.ReturnId == chosenAction.ReturnId)))
                {
                    if ("NextPage".Equals(nextAction.Action, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);
                        if (nextPage != null)
                        {
                            if (nextPage.ActivatedByPageId != null && nextPage.ActivatedByPageId.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(branchingPageId))
                            {
                                nextPage.Active = false;
                            }

                            foreach (var nextPagesAction in nextPage.Next)
                            {
                                DeactivateDependentPages(nextPagesAction, nextPage.PageId, qnaData, nextPage, true);
                            }
                        }
                    }
                }
            }
        }

        protected void ActivateDependentPages(Next chosenAction, string branchingPageId, QnAData qnaData, Page page)
        {
            if (chosenAction != null && page != null)
            {
                // process sub pages which are the chosen action
                foreach (var nextAction in page.Next.Where(n => n.Action == chosenAction.Action && n.ReturnId == chosenAction.ReturnId))
                {
                    if ("NextPage".Equals(nextAction.Action, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);
                        if (nextPage != null)
                        {
                            if (nextPage.ActivatedByPageId != null && nextPage.ActivatedByPageId.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(branchingPageId))
                            {
                                nextPage.Active = true;

                                foreach (var nextPagesAction in nextPage.Next)
                                {
                                    ActivateDependentPages(nextPagesAction, branchingPageId, qnaData, nextPage);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}