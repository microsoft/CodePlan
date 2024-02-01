using System;

namespace SFA.DAS.QnA.Application.Services
{
    public interface ITagProcessingService
    {
        void ClearDeactivatedTags(Guid applicationId, Guid sectionId);
    }
}