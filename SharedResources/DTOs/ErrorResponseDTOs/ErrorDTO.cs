using SharedResources.Contracts;
using System.Net;

namespace SharedResources.DTOs.ErrorResponseDTOs
{
    public class ErrorDTO : IResponseDTO
    {
        public Exception Exception { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
