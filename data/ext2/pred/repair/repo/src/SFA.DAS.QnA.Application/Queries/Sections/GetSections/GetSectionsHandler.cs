using System.Collections.Generic;
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

namespace SFA.DAS.QnA.Application.Queries.Sections.GetSections

{
    public class GetSectionsHandler : IRequestHandler<GetSectionsRequest, HandlerResponse<List<Section>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly INotRequiredProcessor _notRequiredProcessor;

        public GetSectionsHandler(QnaDataContext dataContext, IMapper mapper, INotRequiredProcessor notRequiredProcessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _notRequiredProcessor = notRequiredProcessor;
        }

        public async Task<HandlerResponse<List<Section>>> Handle(GetSectionsRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<List<Section>>(false, "Application does not exist");

            var sections = await _dataContext.ApplicationSections.AsNoTracking()
                .Where(seq => seq.ApplicationId == request.ApplicationId)
                .ToListAsync(cancellationToken: cancellationToken);

            var mappedSections = _mapper.Map<List<Section>>(sections);

            foreach (var section in mappedSections)
            {
                RemovePages(application, section);
            }

            return new HandlerResponse<List<Section>>(mappedSections);
        }

        private void RemovePages(Data.Entities.Application application, Section section)
        {
            var applicationData = JObject.Parse(application.ApplicationData);

            RemovePagesBasedOnNotRequiredConditions(section, applicationData);
            RemoveInactivePages(section);
        }

        private static void RemoveInactivePages(Section section)
        {
            section.QnAData.Pages.RemoveAll(p => !p.Active);
        }

        private void RemovePagesBasedOnNotRequiredConditions(Section section, JObject applicationData)
        {
            section.QnAData.Pages =
                _notRequiredProcessor.PagesWithoutNotRequired(section.QnAData.Pages, applicationData).ToList();

        }
    }

}