﻿using System.Collections.Generic;
using System.Text.Json.Nodes;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Services
{
    public interface INotRequiredProcessor
    {
        bool NotRequired(IEnumerable<NotRequiredCondition> notRequiredConditions, JObject applicationData);
        IEnumerable<Page> PagesWithoutNotRequired(List<Page> pages, JObject applicationData);

    }
}
