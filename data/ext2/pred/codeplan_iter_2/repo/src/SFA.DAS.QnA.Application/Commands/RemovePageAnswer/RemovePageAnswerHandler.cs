using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.AddPageAnswer;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.RemovePageAnswer
{
    public class RemovePageAnswerHandler : PageHandlerBase, IRequestHandler<RemovePageAnswerRequest, HandlerResponse<RemovePageAnswerResponse>>
    {
        private readonly QnaDataContext _dataContext;

        public RemovePageAnswerHandler(QnaDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<RemovePageAnswerResponse>> Handle(RemovePageAnswerRequest request, CancellationToken cancellationToken)
        {
            await GetSectionAndPage(request.ApplicationId, request.SectionId, request.PageId);

            if (Application == null || Section == null || Page == null)
            {
                return new HandlerResponse<RemovePageAnswerResponse>(false, $"ApplicationId {request.ApplicationId}, Section {request.SectionId} or PageId {request.PageId} does not exist.");
            }

            if (Page.AllowMultipleAnswers == false)
            {
                return new HandlerResponse<RemovePageAnswerResponse>(false, $"ApplicationId {request.ApplicationId}, Section {request.SectionId}, PageId {request.PageId} does not AllowMultipleAnswers ");
            }

            var pageOfAnswers = Page.PageOfAnswers.SingleOrDefault(poa => poa.Id == request.AnswerId);

            if (pageOfAnswers == null)
            {
                return new HandlerResponse<RemovePageAnswerResponse>(false, $"AnswerId {request.AnswerId} does not exist.");
            }

            Page.PageOfAnswers.Remove(pageOfAnswers);

            if (Page.PageOfAnswers.Count == 0)
            {
                Page.Complete = false;
                if (Page.HasFeedback)
                {
                    foreach (var feedback in Page.Feedback.Where(feedback => feedback.IsNew).Select(feedback => feedback))
                    {
                        feedback.IsCompleted = false;
                    }
                }
            }
            else
            {
                MarkFeedbackComplete(Page);
            }

            Section.QnAData = QnaData;



            await _dataContext.SaveChangesAsync(CancellationToken.None);

            return new HandlerResponse<RemovePageAnswerResponse>(new RemovePageAnswerResponse(Page));
        }
    }
}