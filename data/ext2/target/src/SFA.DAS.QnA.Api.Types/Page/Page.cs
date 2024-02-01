using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class Page
    {
        public string PageId { get; set; }
        public Guid? SequenceId { get; set; }
        public Guid? SectionId { get; set; }
        public bool ShowTitleAsCaption { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string InfoText { get; set; }
        public List<Question> Questions { get; set; }
        public List<PageOfAnswers> PageOfAnswers { get; set; }
        public List<Next> Next { get; set; }
        public bool Complete { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public int? Order { get; set; }
        public bool Active { get; set; }
        public bool NotRequired { get; set; }

        public List<NotRequiredCondition> NotRequiredConditions { get; set; }

        public string BodyText { get; set; }

        public PageDetails Details { get; set; }

        public string DisplayType { get; set; }
        public bool IsQuestionAnswered(string questionId)
        {
            var allAnswers = PageOfAnswers.SelectMany(poa => poa.Answers).ToList();
            return allAnswers.Any(a => a.QuestionId == questionId);
        }

        public List<Feedback> Feedback { get; set; }

        [JsonIgnore]
        public bool HasFeedback => Feedback?.Any() ?? false;

        [JsonIgnore]
        public bool HasNewFeedback => HasFeedback && Feedback.Any(f => f.IsNew || !f.IsCompleted);

        [JsonIgnore]
        public bool AllFeedbackIsCompleted => !HasFeedback || Feedback.All(f => f.IsCompleted);

        public string ActivatedByPageId { get; set; }
    }
}