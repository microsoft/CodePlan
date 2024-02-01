using MediatR;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData
{
    public class GetQuestionTagDataRequest : IRequest<HandlerResponse<string>>
    {
        public Guid ApplicationId { get; }
        public string QuestionTag { get; }

        public GetQuestionTagDataRequest(Guid applicationId, string questionTag)
        {
            ApplicationId = applicationId;
            QuestionTag = questionTag;
        }
    }
}
