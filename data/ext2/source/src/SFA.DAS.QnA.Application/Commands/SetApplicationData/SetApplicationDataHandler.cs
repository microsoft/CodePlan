using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.StartApplication;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.SetApplicationData
{
    public class SetApplicationDataHandler : IRequestHandler<SetApplicationDataRequest, HandlerResponse<string>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IApplicationDataValidator _applicationDataValidator;

        public SetApplicationDataHandler(QnaDataContext dataContext, IApplicationDataValidator applicationDataValidator)
        {
            _dataContext = dataContext;
            _applicationDataValidator = applicationDataValidator;
        }
        public async Task<HandlerResponse<string>> Handle(SetApplicationDataRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);

            if (application is null) return new HandlerResponse<string>(success: false, message: "Application does not exist.");

            var workflow = await _dataContext.Workflows.SingleOrDefaultAsync(wf => wf.Id == application.WorkflowId, cancellationToken);

            var serializedApplicationData = JsonConvert.SerializeObject(request.ApplicationData);

            if (!_applicationDataValidator.IsValid(workflow.ApplicationDataSchema, serializedApplicationData))
            {
                return new HandlerResponse<string>(success: false, message: "ApplicationData does not validated against the Project's Schema.");
            }

            application.ApplicationData = serializedApplicationData;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<string>(serializedApplicationData);
        }
    }
}