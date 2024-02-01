using SFA.DAS.QnA.Api.Types;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.Controllers.Deserializer
{
    public static class HandlerResponseDeserializer
    {
        public static object Deserialize(HandlerResponse<string> handlerResponse) { return JsonSerializer.Deserialize<object>(handlerResponse.Value); }
    }
}
