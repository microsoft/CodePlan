using System.Collections.Generic;
using System.Dynamic;

namespace SFA.DAS.QnA.Api.Views
{
    public static class ViewHelpers
    {
        private static bool PropertyExists(dynamic dynamicObject, string name)
        {
            if (dynamicObject is ExpandoObject)
                return ((IDictionary<string, object>)dynamicObject).ContainsKey(name);

            return dynamicObject.GetType().GetProperty(name) != null;
        }

        /// <summary>
        /// Gets the validations from the dynamic Model.
        /// Normally the Model will be a QuestionViewModel and will contain Validations.
        /// If the Question is in FurtherQuestions in a ComplexRadio, then the Model will be a Question and
        /// Validations is a sub-property of Input.
        /// Hence the hack
        /// </summary>
        /// <param name="Model">QuestionViewModel or Question</param>
        /// <returns>A dynamic list of Validations</returns>
        public static dynamic GetValidations(dynamic Model)
        {
            var validations = ViewHelpers.PropertyExists(Model, "Validations")
                ? Model.Validations
                : Model.Input.Validations;
            return validations;
        }

        /// <summary>
        /// Gets the options from the dynamic Model.
        /// Normally the Model will be a QuestionViewModel and will contain Options.
        /// If the Question is in FurtherQuestions in a ComplexRadio, then the Model will be a Question and
        /// Options is a sub-property of Input.
        /// Hence the hack
        /// </summary>
        /// <param name="Model">QuestionViewModel or Question</param>
        /// <returns>A dynamic list of Options</returns>
        public static dynamic GetOptions(dynamic Model)
        {
            var options = ViewHelpers.PropertyExists(Model, "Options")
                ? Model.Options
                : Model.Input.Options;
            return options;
        }
    }
}