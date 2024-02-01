using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.AddPageAnswer
{
    public class AddPageAnswerHandler : PageHandlerBase, IRequestHandler<AddPageAnswerRequest, HandlerResponse<AddPageAnswerResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IAnswerValidator _answerValidator;

        public AddPageAnswerHandler(QnaDataContext dataContext, IAnswerValidator answerValidator) : base(dataContext)
        {
            _dataContext = dataContext;
            _answerValidator = answerValidator;
        }

        public async Task<HandlerResponse<AddPageAnswerResponse>> Handle(AddPageAnswerRequest request, CancellationToken cancellationToken)
        {
            await GetSectionAndPage(request.ApplicationId, request.SectionId, request.PageId);

            if (Application == null || Section == null || Page == null)
            {
                return new HandlerResponse<AddPageAnswerResponse>(false, $"ApplicationId {request.ApplicationId}, Section {request.SectionId} or PageId {request.PageId} does not exist.");
            }

            if (Page.AllowMultipleAnswers == false)
            {
                return new HandlerResponse<AddPageAnswerResponse>(false, $"ApplicationId {request.ApplicationId}, Section {request.SectionId}, PageId {request.PageId} does not AllowMultipleAnswers");
            }

            if (Page.PageOfAnswers == null)
            {
                Page.PageOfAnswers = new List<PageOfAnswers>();
            }

            var validationErrors = _answerValidator.Validate(request.Answers, Page);
            if (validationErrors.Any())
            {
                return new HandlerResponse<AddPageAnswerResponse>(new AddPageAnswerResponse(validationErrors));
            }

            Page.PageOfAnswers.Add(new PageOfAnswers() { Id = Guid.NewGuid(), Answers = request.Answers });

            Page.Complete = true;

            MarkFeedbackComplete(Page);

            Section.QnAData = QnaData;

            await _dataContext.SaveChangesAsync(CancellationToken.None);

            return new HandlerResponse<AddPageAnswerResponse>(new AddPageAnswerResponse(Page));
        }
    }
}