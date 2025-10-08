using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class IdentityResponse<TDto> : IResponseTypes<TDto, IdentityResponseMesage>
       where TDto : class, IIdentityResponseDTO
    {
        public static IdentityResponse<TDto> _identityResponse { get => new IdentityResponse<TDto>(); }


        public TDto Data { get; set; }
        public bool IsSuccess { get; set; }
        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string _message { get; set; }


        public IResponseTypes<TDto, IdentityResponseMesage> CreateSuccessResponse(
        TDto data, IdentityResponseMesage message, HttpStatusCode statusCode)
        {

            _identityResponse.Data = data;
            _identityResponse._message = message.MessageOutput;
            _identityResponse.IsSuccess = true;
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return _identityResponse;
        }

        public IResponseTypes<TDto, IdentityResponseMesage> CreateErrorResponse(
            string errorMessage, HttpStatusCode statusCode)
        {

            _identityResponse._message = errorMessage;
            _identityResponse.IsSuccess = false;
            _identityResponse.CustomError = errorMessage;
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return _identityResponse;
        }

        public IResponseTypes<TDto, IdentityResponseMesage> CreateErrorResponse(
            IdentityResponseMesage responseMessage, HttpStatusCode statusCode, List<string> errors = null)
        {

            _identityResponse.IsSuccess = false;
            _identityResponse._message = responseMessage.MessageOutput;
            _identityResponse.Errors = errors ?? new List<string>();
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return _identityResponse;
        }
    }
}
