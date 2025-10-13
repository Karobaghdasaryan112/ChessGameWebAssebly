using SharedResources.Contracts;
using SharedResources.Contracts.DTOs;
using System.Net;
using System.Text.Json.Serialization;

namespace SharedResources.DTOs.ErrorResponseDTOs
{
    public class IdentityErrorDTO : IIdentityResponseDTO
    {
        [JsonConstructor]
        public IdentityErrorDTO(Exception exception,HttpStatusCode httpStatusCode)
        {
            Exception = exception;
            StatusCode = httpStatusCode;
        }
        public Exception Exception { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int UserId { get ; set ; }
    }
}
