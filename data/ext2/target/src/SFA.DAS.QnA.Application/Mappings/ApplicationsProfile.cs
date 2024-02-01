using AutoMapper;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Mappings
{
    public class ApplicationsProfile : Profile
    {
        public ApplicationsProfile()
        {
            CreateMap<ApplicationSequence, Sequence>();
            CreateMap<ApplicationSection, Section>();
        }
    }
}