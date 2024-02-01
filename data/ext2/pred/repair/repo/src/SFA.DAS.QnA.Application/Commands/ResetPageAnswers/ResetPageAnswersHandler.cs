using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetPageAnswersHandler : SetAnswersBase, IRequestHandler<ResetPageAnswersRequest, HandlerResponse<ResetPageAnswersResponse>>
    {
        public ResetPageAnswersHandler(QnaDataContext dataContext, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, null)
        {
        }

        public async Task<HandlerResponse<ResetPageAnswersResponse>> Handle(ResetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var validationErrorResponse = ValidateResetPageAnswersRequest(request.PageId, section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            await ResetPageAnswers(request.PageId, request.ApplicationId, section, cancellationToken);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<ResetPageAnswersResponse>(new ResetPageAnswersResponse(true));
        }
    }
}
