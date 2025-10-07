using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.Responses
{
    public class IdentityResponse<TDto> : IResponseTypes<TDto, IdentityResponseMesage>
       where TDto : class, IIdentityDTO
    {
        private static IdentityResponse<TDto> _identityResponse = new IdentityResponse<TDto>();
        private static IdentityResponseMesage _identityResponseMesage = new IdentityResponseMesage(default);


        public static IdentityResponse<TDto> identityResponse => _identityResponse ?? new IdentityResponse<TDto>();
        public static IdentityResponseMesage IdentityResponseMesage
        {
            get => _identityResponseMesage ??= new IdentityResponseMesage(default);
            set => _identityResponseMesage = value;
        }

        public TDto Data { get; set; }
        public bool IsSuccess { get; set; }

        public string CustomError { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string _message { get; set ; }

        public Task<IResponseTypes<TDto, IdentityResponseMesage>> CreateSuccessResponse(
        TDto data, IdentityResponseMesage message, HttpStatusCode statusCode) 
        {
            _identityResponse.Data = data;
            _identityResponse._message = message.messageOutput;
            _identityResponse.IsSuccess = true;
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return Task.FromResult<IResponseTypes<TDto, IdentityResponseMesage>>(_identityResponse);
        }

        public async Task<IResponseTypes<TDto, IdentityResponseMesage>> CreateErrorResponse(
            string errorMessage, HttpStatusCode statusCode)
        {
            _message = errorMessage;
            _identityResponse.IsSuccess = false;
            _identityResponse.CustomError = errorMessage;
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return _identityResponse;
        }

        public async Task<IResponseTypes<TDto, IdentityResponseMesage>> CreateErrorResponse(
            IdentityResponseMesage responseMessage, HttpStatusCode statusCode, List<string> errors = null)
        {
            _identityResponse.IsSuccess = false;
            _message = responseMessage.messageOutput;
            _identityResponse.Errors = errors ?? new List<string>();
            _identityResponse.StatusCode = statusCode;
            _identityResponse.Timestamp = DateTime.UtcNow;
            return _identityResponse;
        }
    }
}
