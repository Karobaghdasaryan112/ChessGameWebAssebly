using SharedResources.Contracts;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class ErrorResponse<TDto> : IResponseTypes<TDto, ErrorResponseMessage>
        where TDto : class, IResponseDTO
    {

        public TDto Data { get; set; }
        public bool IsSuccess { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ErrorResponseMessage message { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public IdentityErrorDTO? IdentityErrorDTO { get; set; }

        public static IResponseTypes<TDto, ErrorResponseMessage> CreateErrorResponse(TDto errorDto,
            ErrorResponseMessage responseMessage, HttpStatusCode statusCode)
        {
            return new ErrorResponse<TDto>
            {
                Data = errorDto,
                StatusCode = statusCode,
                message = responseMessage,
            };
        }
    }
}
