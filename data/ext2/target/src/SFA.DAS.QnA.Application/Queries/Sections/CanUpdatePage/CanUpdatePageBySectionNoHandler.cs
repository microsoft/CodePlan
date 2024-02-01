using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage
{
    public class CanUpdatePageBySectionNoHandler : IRequestHandler<CanUpdatePageBySectionNoRequest, HandlerResponse<bool>>
    {
        private readonly QnaDataContext _dataContext;

        public CanUpdatePageBySectionNoHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<bool>> Handle(CanUpdatePageBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.SectionNo == request.SectionNo && sec.SequenceNo == request.SequenceNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<bool>(false, "Section does not exist");

            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<bool>(false, "Page does not exist");

            return new HandlerResponse<bool>(page.Active);
        }
    }
}