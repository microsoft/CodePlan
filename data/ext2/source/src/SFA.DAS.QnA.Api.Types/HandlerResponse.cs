namespace SFA.DAS.QnA.Api.Types
{
    public class HandlerResponse<TResponseType>
    {
        public HandlerResponse(TResponseType value)
        {
            Value = value;
        }

        public HandlerResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public HandlerResponse() { }

        public TResponseType Value { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}