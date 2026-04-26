using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using System.Text.Json.Serialization;

namespace SharedResources.Responses
{
    public class IdentityResponse<TDto> : ResponseDTO<TDto,ChessGameResponseMessage>
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

        public static ResponseDTO<TDto, ChessGameResponseMessage> CreateSuccessResponse(
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

        public static ResponseDTO<TDto, ChessGameResponseMessage> CreateErrorResponse(
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
