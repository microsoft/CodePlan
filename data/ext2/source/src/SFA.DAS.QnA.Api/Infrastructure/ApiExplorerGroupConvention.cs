using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    public class ApiExplorerGroupConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            if (controllerNamespace.Contains("Config"))
            {
                controller.ApiExplorer.GroupName = "config";
            }
            else
            {
                controller.ApiExplorer.GroupName = "v1";
            }
        }
    }
}