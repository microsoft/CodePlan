using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData
{
    public class GetApplicationDataHandler : IRequestHandler<GetApplicationDataRequest, HandlerResponse<string>>
    {
        private readonly QnaDataContext _dataContext;

        public GetApplicationDataHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<string>> Handle(GetApplicationDataRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            return application is null
                ? new HandlerResponse<string>(success: false, message: "Application does not exist.")
                : new HandlerResponse<string>(application.ApplicationData);
        }


    }
}