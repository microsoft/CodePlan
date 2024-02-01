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

    public class ResetPageAnswersBySectionNoHandler : SetAnswersBase, IRequestHandler<ResetPageAnswersBySectionNoRequest, HandlerResponse<ResetPageAnswersResponse>>
    {
        public ResetPageAnswersBySectionNoHandler(QnaDataContext dataContext, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, null)
        {
        }

        public async Task<HandlerResponse<ResetPageAnswersResponse>> Handle(ResetPageAnswersBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.SequenceNo == request.SequenceNo && sec.SectionNo == request.SectionNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
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
