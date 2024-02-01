using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequence
{
    public class GetSequenceBySequenceNoHandler : IRequestHandler<GetSequenceBySequenceNoRequest, HandlerResponse<Sequence>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequenceBySequenceNoHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<HandlerResponse<Sequence>> Handle(GetSequenceBySequenceNoRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<Sequence>(false, "Application does not exist");

            var sequence = await _dataContext.ApplicationSequences.AsNoTracking().FirstOrDefaultAsync(seq => seq.SequenceNo == request.SequenceNo && seq.ApplicationId == request.ApplicationId, cancellationToken: cancellationToken);
            if (sequence is null) return new HandlerResponse<Sequence>(false, "Sequence does not exist");

            return new HandlerResponse<Sequence>(_mapper.Map<Sequence>(sequence));
        }
    }
}