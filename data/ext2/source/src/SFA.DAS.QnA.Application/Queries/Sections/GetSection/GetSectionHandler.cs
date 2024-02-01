using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSection
{
    public class GetSectionHandler : IRequestHandler<GetSectionRequest, HandlerResponse<Section>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly INotRequiredProcessor _notRequiredProcessor;

        public GetSectionHandler(QnaDataContext dataContext, IMapper mapper, INotRequiredProcessor notRequiredProcessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _notRequiredProcessor = notRequiredProcessor;
        }

        public async Task<HandlerResponse<Section>> Handle(GetSectionRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<Section>(false, "Application does not exist");

            var section = await _dataContext.ApplicationSections.AsNoTracking().FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            if (section is null) return new HandlerResponse<Section>(false, "Section does not exist");

            RemovePages(application, section);

            return new HandlerResponse<Section>(_mapper.Map<Section>(section));
        }

        private void RemovePages(Data.Entities.Application application, ApplicationSection section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);

            RemoveInactivePages(section);
            RemovePagesBasedOnNotRequiredConditions(section, applicationData);
        }

        private static void RemoveInactivePages(ApplicationSection section)
        {
            section.QnAData.Pages.RemoveAll(p => !p.Active);
        }

        private void RemovePagesBasedOnNotRequiredConditions(ApplicationSection section, JObject applicationData)
        {
            section.QnAData.Pages =
               _notRequiredProcessor.PagesWithoutNotRequired(section.QnAData.Pages, applicationData).ToList();
        }
    }
}