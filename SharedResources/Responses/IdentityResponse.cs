using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using System.Text.Json.Serialization;

namespace SharedResources.Responses
{
    public class IdentityResponse<TDto> : IResponseTypes<TDto, IdentityResponseMesage>
       where TDto : class, IIdentityResponseDTO
    {
        [JsonConstructor]
        public IdentityResponse()
        {

        }

        public TDto? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string CustomError { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public IdentityResponseMesage message { get; set; } = IdentityResponseMesage.Initialize;
        public IdentityErrorDTO? IdentityErrorDTO { get; set; }

        public static IResponseTypes<TDto, IdentityResponseMesage> CreateSuccessResponse(
        TDto data, IdentityResponseMesage message, HttpStatusCode statusCode)
        {
            return new IdentityResponse<TDto>
            {
                Data = data,
                message = message,
                IsSuccess = true,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow,
            };
        }

        public static IResponseTypes<TDto, IdentityResponseMesage> CreateErrorResponse(
            IdentityResponseMesage responseMessage, HttpStatusCode statusCode, List<string> errors)
        {
            return new IdentityResponse<TDto>
            {
                message = responseMessage,
                IsSuccess = false,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow,
                Errors = errors
            };

        }
    }
}
