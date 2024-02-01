using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class BadRequestError : ApiError
    {
        public BadRequestError()
            : base(400, HttpStatusCode.BadRequest.ToString())
        {
        }


        public BadRequestError(string message)
            : base(400, HttpStatusCode.BadRequest.ToString(), message)
        {
        }
    }
}