using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class ConnectionResponseDTO<TDto, TMessage> where TMessage : ChessGameResponseMessage
    {
        public TDto Data;
        public TMessage Message;
        public HttpStatusCode HttpStatusCode;
        public List<string> Errors;
        public bool IsSuccess;

        public static ConnectionResponseDTO<TDto, TMessage> CreateErrorResponse(
            TDto responseDTO,
            TMessage errorMessage,
            HttpStatusCode httpStatusCode = default,
            List<string> errors = default)
        {
            return new ConnectionResponseDTO<TDto, TMessage>()
            {
                IsSuccess = false,
                Data = responseDTO,
                Message = errorMessage,
                HttpStatusCode = httpStatusCode,
                Errors = errors
            };
        }

        public static ConnectionResponseDTO<TDto, TMessage> CreateSuccessResponse(
            TDto responseDTO,
            TMessage message,
            HttpStatusCode httpStatusCode = default)
        {
            return new ConnectionResponseDTO<TDto, TMessage>()
            {
                IsSuccess = true,
                Data = responseDTO,
                Message = message,
                HttpStatusCode = httpStatusCode,
            };
        }
    }
}
