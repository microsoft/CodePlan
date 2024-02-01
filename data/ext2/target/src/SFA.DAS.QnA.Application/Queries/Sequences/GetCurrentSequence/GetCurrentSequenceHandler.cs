using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence
{
    public class GetCurrentSequenceHandler : IRequestHandler<GetCurrentSequenceRequest, HandlerResponse<Sequence>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetCurrentSequenceHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<HandlerResponse<Sequence>> Handle(GetCurrentSequenceRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            if (application is null) return new HandlerResponse<Sequence>(false, "Application does not exist");

            var currentSequence = await _dataContext.ApplicationSequences.AsNoTracking().FirstOrDefaultAsync(seq => seq.ApplicationId == request.ApplicationId && seq.IsActive, cancellationToken);

            return new HandlerResponse<Sequence>(_mapper.Map<Sequence>(currentSequence));
        }
    }
}