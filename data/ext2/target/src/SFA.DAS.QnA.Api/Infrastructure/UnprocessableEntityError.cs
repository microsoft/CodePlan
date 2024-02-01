using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class UnprocessableEntityError : ApiError
    {
        public UnprocessableEntityError()
            : base(422, HttpStatusCode.UnprocessableEntity.ToString())
        {
        }
        public UnprocessableEntityError(string message)
            : base(422, HttpStatusCode.UnprocessableEntity.ToString(), message)
        {
        }
    }
}
