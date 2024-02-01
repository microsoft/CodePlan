using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetPage
{
    public class GetPageHandler : IRequestHandler<GetPageRequest, HandlerResponse<Page>>
    {
        private readonly QnaDataContext _dataContext;

        public GetPageHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<Page>> Handle(GetPageRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.AsNoTracking().FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<Page>(false, "Section does not exist");

            var page = section.QnAData.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            if (page is null) return new HandlerResponse<Page>(false, "Page does not exist");

            return new HandlerResponse<Page>(page);
        }
    }
}