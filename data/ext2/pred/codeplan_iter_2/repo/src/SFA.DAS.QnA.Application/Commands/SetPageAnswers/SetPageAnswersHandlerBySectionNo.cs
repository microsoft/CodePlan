using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersBySectionNoHandler : SetAnswersBase, IRequestHandler<SetPageAnswersBySectionNoRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        public SetPageAnswersBySectionNoHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator)
        {
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.SequenceNo == request.SequenceNo && sec.SectionNo == request.SectionNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var validationErrorResponse = ValidateSetPageAnswersRequest(request.PageId, request.Answers, section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            SaveAnswersIntoPage(section, request.PageId, request.Answers);

            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            UpdateApplicationData(request.PageId, request.Answers, section, application);

            var nextAction = GetNextActionForPage(section, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}