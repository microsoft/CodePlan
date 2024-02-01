using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Services
{
    public class NotRequiredProcessor : INotRequiredProcessor
    {
        public bool NotRequired(IEnumerable<NotRequiredCondition> notRequiredConditions, JObject applicationData)
        {
            return notRequiredConditions.Any(nrc =>
                nrc.IsOneOf != null && nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>()));
        }

        public IEnumerable<Page> PagesWithoutNotRequired(List<Page> pages, JObject applicationData)
        {
            pages.RemoveAll(p => p.NotRequiredConditions != null &&
                                 p.NotRequiredConditions.Any(nrc => nrc.IsOneOf != null && nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>())));

            var pagesToRemove = new List<string>();
            foreach (var page in pages.Where(p => p.NotRequiredConditions != null))
            {
                if (page.NotRequiredConditions == null) continue;
                foreach (var notRequiredCondition in page.NotRequiredConditions.Where(n => n.ContainsAllOf != null && n.ContainsAllOf.Any()))
                {
                    var fieldToCheck = notRequiredCondition.Field;
                    var fieldValue = applicationData[fieldToCheck]?.Value<string>();
                    if (string.IsNullOrEmpty(fieldValue)) continue;

                    var applicationDataValues = fieldValue.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    var containsAllValues =
                        applicationDataValues.Select(p => p)
                            .Intersect(notRequiredCondition.ContainsAllOf.Distinct()).Count() ==
                        notRequiredCondition.ContainsAllOf.Distinct().Count();

                    if (containsAllValues)
                        pagesToRemove.Add(page.PageId);
                }

                foreach (var notRequiredCondition in page.NotRequiredConditions.Where(n =>
                    n.DoesNotContain != null && n.DoesNotContain.Any(x => !string.IsNullOrEmpty(x))))
                {
                    var isPresent = false;
                    var fieldToCheck = notRequiredCondition.Field;
                    var fieldValue = applicationData[fieldToCheck]?.Value<string>();
                    if (!string.IsNullOrEmpty(fieldValue))
                    {

                        var applicationDataValues = fieldValue.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        foreach (var adv in applicationDataValues)
                        {
                            foreach (var condition in notRequiredCondition.DoesNotContain)
                            {
                                if (string.Equals(adv, condition, StringComparison.InvariantCultureIgnoreCase)) isPresent = true;
                            }
                        }

                    }

                    if (!isPresent)
                        pagesToRemove.Add(page.PageId);
                }
            }

            pages.RemoveAll(p => pagesToRemove.Any(pr => pr == p.PageId));


            return pages;
        }
    }
}
