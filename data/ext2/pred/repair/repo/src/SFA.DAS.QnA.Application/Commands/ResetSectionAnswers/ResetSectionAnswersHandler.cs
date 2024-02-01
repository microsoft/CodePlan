using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Commands.Files.DeleteFile;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Application.Commands.ResetPageAnswers
{
    public class ResetSectionAnswersHandler : SetAnswersBase, IRequestHandler<ResetSectionAnswersRequest, HandlerResponse<ResetSectionAnswersResponse>>
    {
        private readonly IMediator _mediator;

        public ResetSectionAnswersHandler(QnaDataContext dataContext, IMediator mediator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService)
            : base(dataContext, notRequiredProcessor, tagProcessingService, null)
        {
            _mediator = mediator;
        }

        public async Task<HandlerResponse<ResetSectionAnswersResponse>> Handle(ResetSectionAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.SequenceNo == request.SequenceNo && sec.SectionNo == request.SectionNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var validationErrorResponse = ValidateSectionAnswersRequest(section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            foreach (var page in section.QnAData.Pages)
            {
                if (page.Questions.Any(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    foreach (var fileUploadQuestion in page.Questions.Where(q => "FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var answer = page.PageOfAnswers
                            .Select(x => x.Answers
                                .FirstOrDefault(b => b.QuestionId == fileUploadQuestion.QuestionId))
                            .FirstOrDefault();

                        if (answer != null)
                        {
                            await _mediator.Send(new DeleteFileRequest(request.ApplicationId, section.Id, page.PageId, fileUploadQuestion.QuestionId, answer.Value), CancellationToken.None);
                        }
                    }
                }

                await ResetPageAnswers(page.PageId, request.ApplicationId, section, cancellationToken, true, true);
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<ResetSectionAnswersResponse>(new ResetSectionAnswersResponse(true));
        }
    }
}
