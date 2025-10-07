using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class ChatResponse : IResponseTypes<IChatDTO, ChatResponseMessage>
    {
        public bool IsSuccess { get; set; }
        public ChatResponseMessage Message { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public string _message { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IChatDTO IResponseTypes<IChatDTO, ChatResponseMessage>.Data { get; set; }

        public Task<IResponseTypes<IChatDTO, ChatResponseMessage>> CreateErrorResponse(
            string errorMessage,
            HttpStatusCode statusCode)
        {
            this.CustomError = errorMessage;
            this.StatusCode = statusCode;
            this.IsSuccess = false;
            this.Timestamp = DateTime.UtcNow;
            return Task.FromResult<IResponseTypes<IChatDTO, ChatResponseMessage>>(this);
        }

        public Task<IResponseTypes<IChatDTO, ChatResponseMessage>> CreateErrorResponse(
            ChatResponseMessage responseMessage,
            HttpStatusCode statusCode,
            List<string> errors)
        {
            this.Message = responseMessage;
            this.StatusCode = statusCode;
            this.Errors = errors;
            this.IsSuccess = false;
            this.Timestamp = DateTime.UtcNow;
            return Task.FromResult<IResponseTypes<IChatDTO, ChatResponseMessage>>(this);
        }

        public Task<IResponseTypes<IChatDTO, ChatResponseMessage>> CreateSuccessResponse(
            IChatDTO data,
            ChatResponseMessage message,
            HttpStatusCode statusCode)
        {
            this.Message = message;
            ((IResponseTypes<IChatDTO, ChatResponseMessage>)this).Data = data;
            this.StatusCode = statusCode;
            this.IsSuccess = true;
            this.Timestamp = DateTime.UtcNow;
            return Task.FromResult<IResponseTypes<IChatDTO, ChatResponseMessage>>(this);
        }
    }
}
