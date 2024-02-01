using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.QnA.Application.Services;
using System.Text.Json.Nodes;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSection
{
    public class GetSectionBySectionNoHandler : IRequestHandler<GetSectionBySectionNoRequest, HandlerResponse<Section>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly INotRequiredProcessor _notRequiredProcessor;

        public GetSectionBySectionNoHandler(QnaDataContext dataContext, IMapper mapper, INotRequiredProcessor notRequiredProcessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _notRequiredProcessor = notRequiredProcessor;
        }

        public async Task<HandlerResponse<Section>> Handle(GetSectionBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<Section>(false, "Application does not exist");

            var section = await _dataContext.ApplicationSections.AsNoTracking().FirstOrDefaultAsync(sec => sec.SectionNo == request.SectionNo && sec.SequenceNo == request.SequenceNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<Section>(false, "Section does not exist");

            RemovePages(application, section);

            return new HandlerResponse<Section>(_mapper.Map<Section>(section));
        }

        private void RemovePages(Data.Entities.Application application, ApplicationSection section)
        {
            var applicationData = JsonNode.Parse(application.ApplicationData);

            RemovePagesBasedOnNotRequiredConditions(section, applicationData);
            RemoveInactivePages(section);
        }

        private static void RemoveInactivePages(ApplicationSection section)
        {
            section.QnAData.Pages.RemoveAll(p => !p.Active);
        }

        private void RemovePagesBasedOnNotRequiredConditions(ApplicationSection section, JsonNode applicationData)
        {
            section.QnAData.Pages =
                _notRequiredProcessor.PagesWithoutNotRequired(section.QnAData.Pages, applicationData.AsObject()).ToList();
        }
    }
}