using SharedResources.Contracts.RequestsAndResponses;
using System.Net;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs
{
    public class ResponseDTO<TDto, TMessage> where TMessage : IMessage
    {
        public TDto Data { get; set; }
        public List<TDto> Datas { get; set; }

        public TMessage Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public string CustomError { get; set; } = string.Empty;
        public List<string> Errors { get; set; }

        public bool IsSuccess { get; set; }

        public static ResponseDTO<TDto, TMessage> CreateErrorResponse(TDto responseDTO, TMessage errorMessage, HttpStatusCode httpStatusCode = default, List<string> errors = default)
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

        public static ResponseDTO<TDto, TMessage> CreateSuccessResponse(TDto responseDTO, TMessage message, HttpStatusCode httpStatusCode = default)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                IsSuccess = true,
                Data = responseDTO,
                Message = message,
                HttpStatusCode = httpStatusCode,
            };
        }
        public ResponseDTO<TDto, TMessage> CreateErrorResponse(string errorMessage, HttpStatusCode statusCode)
        {
            return new ResponseDTO<TDto, TMessage>
            {
                Data = default,
                IsSuccess = false,
                CustomError = errorMessage,
                HttpStatusCode = statusCode

            };
        }

        public static ResponseDTO<TDto, TMessage> CreateErrorResponse(TMessage responseMessage, HttpStatusCode statusCode, List<string> errors = null)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                Data = default,
                IsSuccess = false,
                Errors = errors,
                HttpStatusCode = statusCode
            };
        }

        public static ResponseDTO<TDto, TMessage> CreateSuccessResponse(TDto data,
            TMessage message, HttpStatusCode statusCode, object unknown)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                HttpStatusCode = statusCode
            };
        }
        public static ResponseDTO<TDto, TMessage> CreateSuccessResponse(List<TDto> datas, TMessage message, HttpStatusCode statusCode)
        {
            return new ResponseDTO<TDto, TMessage>()
            {
                Datas = datas,
                IsSuccess = true,
                Message = message,
                HttpStatusCode = statusCode
            };
        }
    }
}
