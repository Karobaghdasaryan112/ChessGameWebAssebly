using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs
{
    public class ResponseDTO<TDto, TMessage> where TMessage : ChessGameResponseMessage
    {
        public TDto Data { get; set; }
        public TMessage Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public List<string> Errors { get; set; }
        public bool IsSuccess { get; set; }

        public static ResponseDTO<TDto, TMessage> CreateErrorResponse(
            TDto responseDTO,
            TMessage errorMessage,
            HttpStatusCode httpStatusCode = default,
            List<string> errors = default)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                IsSuccess = false,
                Data = responseDTO,
                Message = errorMessage,
                HttpStatusCode = httpStatusCode,
                Errors = errors
            };
        }

        public static ResponseDTO<TDto, TMessage> CreateSuccessResponse(
            TDto responseDTO,
            TMessage message,
            HttpStatusCode httpStatusCode = default)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                IsSuccess = true,
                Data = responseDTO,
                Message = message,
                HttpStatusCode = httpStatusCode,
            };
        }
    }
}
