using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class ChatResponse<TDto> : ResponseDTO<TDto, ChatResponseMessage>
    {
        public bool IsSuccess { get; set; }
        public ChatResponseMessage Message { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public TDto Data { get ; set ; }

        public static ChatResponse<TDto> _chatResponse { get => new ChatResponse<TDto>(); }
        public ChatResponseMessage message { get ; set ; }
        public IdentityErrorDTO? IdentityErrorDTO { get ; set ; }

        public ResponseDTO<TDto, ChatResponseMessage> CreateErrorResponse(
            string errorMessage,
            HttpStatusCode statusCode)
        {
            _chatResponse.CustomError = errorMessage;
            _chatResponse.StatusCode = statusCode;
            _chatResponse.IsSuccess = false;
            _chatResponse.Timestamp = DateTime.UtcNow;
            return _chatResponse;
        }

        public ResponseDTO<TDto, ChatResponseMessage> CreateErrorResponse(
            ChatResponseMessage responseMessage,
            HttpStatusCode statusCode,
            List<string> errors)
        {
            _chatResponse.Message = responseMessage;
            _chatResponse.StatusCode = statusCode;
            _chatResponse.Errors = errors;
            _chatResponse.IsSuccess = false;
            _chatResponse.Timestamp = DateTime.UtcNow;
            return _chatResponse;
        }

        public ResponseDTO<TDto, ChatResponseMessage> CreateSuccessResponse(
            TDto data,
            ChatResponseMessage message,
            HttpStatusCode statusCode)
        {
            _chatResponse.Message = message;
            _chatResponse.Data = data;
            _chatResponse.StatusCode = statusCode;
            _chatResponse.IsSuccess = true;
            _chatResponse.Timestamp = DateTime.UtcNow;
            return _chatResponse;
        }

    }
}
