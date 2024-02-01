using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.QnA.Application.Commands.PageFeedback.CompleteFeedbackWithinSequence
{
    public class CompleteFeedbackWithinSequenceHandler : IRequestHandler<CompleteFeedbackWithinSequenceRequest, HandlerResponse<bool>>
    {
        private readonly QnaDataContext _dataContext;

        public CompleteFeedbackWithinSequenceHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<bool>> Handle(CompleteFeedbackWithinSequenceRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<bool>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.FirstOrDefaultAsync(seq => seq.Id == request.SequenceId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<bool>(false, "Sequence does not exist");

            var sections = await _dataContext.ApplicationSections.Where(section => section.SequenceId == request.SequenceId).ToListAsync(cancellationToken);

            foreach (var section in sections)
            {
                var qnaData = new QnAData(section.QnAData);

                foreach (var page in qnaData.Pages)
                {
                    if (page.HasNewFeedback)
                    {
                        page.Feedback.ForEach(f => f.IsNew = false);
                        page.Feedback.ForEach(f => f.IsCompleted = true);
                    }
                }

                section.QnAData = qnaData;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<bool>(true);
        }
    }
}
