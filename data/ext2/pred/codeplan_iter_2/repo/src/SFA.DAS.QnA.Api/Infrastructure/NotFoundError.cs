using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class NotFoundError : ApiError
    {
        public NotFoundError()
            : base(404, HttpStatusCode.NotFound.ToString())
        {
        }


        public NotFoundError(string message)
            : base(404, HttpStatusCode.NotFound.ToString(), message)
        {
        }
    }
}