using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.ResetPagesToIncomplete
{
    public class ResetPagesToIncompleteHandler : SetAnswersBase, IRequestHandler<ResetPagesToIncompleteRequest, HandlerResponse<bool>>
    {
        private readonly ILogger<ResetPagesToIncompleteHandler> _logger;
        public ResetPagesToIncompleteHandler(QnaDataContext dataContext, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService, ILogger<ResetPagesToIncompleteHandler> logger) : base(dataContext, notRequiredProcessor, tagProcessingService, null)
        {
            _logger = logger;
        }

        public async Task<HandlerResponse<bool>> Handle(ResetPagesToIncompleteRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.ApplicationId == request.ApplicationId && sec.SequenceNo == request.SequenceNo && sec.SectionNo == request.SectionNo, cancellationToken);
            if (section == null) return new HandlerResponse<bool>(true);
            var qnaData = new QnAData(section.QnAData);
            if (qnaData?.Pages == null) return new HandlerResponse<bool>(true);
            var updateMade = false;

            foreach (var page in qnaData.Pages)
            {
                if (request.PageIdsToExclude.Contains(page.PageId)) continue;
                if (!page.Complete) continue;
                page.Complete = false;
                _logger.LogInformation($"Reset applicationId {request.ApplicationId} PageId {page.PageId} to complete=false");
                updateMade = true;
            }

            if (!updateMade) return new HandlerResponse<bool>(true);

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<bool>(true);
        }
    }
}
