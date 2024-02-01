using System;
using System.Collections.Generic;

namespace SFA.DAS.QnA.Api.Types.Page
{
    public class PageOfAnswers
    {
        public Guid Id { get; set; }
        public List<Answer> Answers { get; set; }
    }
}