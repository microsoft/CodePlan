using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public interface IApplicationDataValidator
    {
        bool IsValid(string projectApplicationDataSchema, string applicationData);
    }

    public class ApplicationDataValidator : IApplicationDataValidator
    {
        public bool IsValid(string projectApplicationDataSchema, string applicationData)
        {
            var schema = JSchema.Parse(projectApplicationDataSchema);

            var applicationDataObject = JObject.Parse(applicationData);

            return applicationDataObject.IsValid(schema);
        }
    }
}