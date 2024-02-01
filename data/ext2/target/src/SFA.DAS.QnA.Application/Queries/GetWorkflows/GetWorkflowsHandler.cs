using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.GetWorkflows
{
    public class GetWorkflowsHandler : IRequestHandler<GetWorkflowsRequest, HandlerResponse<List<Workflow>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetWorkflowsHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<HandlerResponse<List<Workflow>>> Handle(GetWorkflowsRequest request, CancellationToken cancellationToken)
        {
            var workflows = await _dataContext.Workflows.AsNoTracking().Where(w => w.Status == WorkflowStatus.Live).ToListAsync(cancellationToken: cancellationToken);

            var responses = _mapper.Map<List<Workflow>>(workflows);

            return new HandlerResponse<List<Workflow>>(responses);
        }
    }
}