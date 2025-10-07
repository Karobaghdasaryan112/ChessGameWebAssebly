using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class ChessGameResponse : IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>
    {
        public ICheseGameDTO Data { get; set; }
        public bool IsSuccess { get; set; }
        public ChessGameResponseMessage Message { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public string _message { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>> CreateErrorResponse(
            string errorMessage,
            HttpStatusCode statusCode)
        {
            this.Data = default;
            this.IsSuccess = false;
            this.CustomError = errorMessage;
            this.StatusCode = statusCode;
            return Task.FromResult<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>>(this);
        }

        public Task<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>> CreateErrorResponse(
            ChessGameResponseMessage responseMessage,
            HttpStatusCode statusCode,
            List<string> errors = null)
        {
            this.Data = default;
            this.IsSuccess = false;
            this.Message = responseMessage;
            this.StatusCode = statusCode;
            this.Errors = errors ?? new List<string>();
            return Task.FromResult<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>>(this);
        }

        public Task<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>> CreateSuccessResponse(
            ICheseGameDTO data,
            ChessGameResponseMessage message,
            HttpStatusCode statusCode)
        {
            this.Data = data;
            this.IsSuccess = true;
            this.Message = message;
            this.StatusCode = statusCode;
            return Task.FromResult<IResponseTypes<ICheseGameDTO, ChessGameResponseMessage>>(this);
        }
    }
}
