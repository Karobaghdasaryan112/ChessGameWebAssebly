using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class ChessGameResponse<TDto> : IResponseTypes<TDto, ChessGameResponseMessage>
        where TDto : ICheseGameResponseDTO
    {
        public TDto Data { get; set; }
        public bool IsSuccess { get; set; }
        public ChessGameResponseMessage Message { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }

        public ChessGameResponseMessage message { get; set; }
        public IdentityErrorDTO? IdentityErrorDTO { get; set; }

        public IResponseTypes<TDto, ChessGameResponseMessage> CreateErrorResponse(
        string errorMessage,HttpStatusCode statusCode)
        {
            return new ChessGameResponse<TDto>
            {
                Data = default,
                IsSuccess = false,
                CustomError = errorMessage,
                StatusCode = statusCode

            };
        }

        public static IResponseTypes<TDto, ChessGameResponseMessage> CreateErrorResponse(
        ChessGameResponseMessage responseMessage,HttpStatusCode statusCode,List<string> errors = null)
            {
                return new ChessGameResponse<TDto>()
                {
                    Data = default,
                    IsSuccess = false,
                    Errors = errors,
                    Message = responseMessage,
                    StatusCode = statusCode
                };
            }

        public static IResponseTypes<TDto, ChessGameResponseMessage> CreateSuccessResponse(
            TDto data,ChessGameResponseMessage message,HttpStatusCode statusCode)
        {
            return new ChessGameResponse<TDto>()
            {
                Data = data,
                IsSuccess = true,
                message = message,
                StatusCode = statusCode
            };
        }
    }
}
