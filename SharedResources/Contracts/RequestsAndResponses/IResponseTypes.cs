using SharedResources.Contracts.DTOs;
using System.Net;

namespace SharedResources.Contracts.RequestsAndResponses
{
    /// <summary>
    ///  
    /// <summary>
    /// Defines a generic contract for API response types, supporting both success and error responses.
    /// Includes properties for response data, status, messages, error details, HTTP status code, and timestamp.
    /// Provides methods to create success and error responses with customizable messages and error lists.
    /// </summary>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface IResponseTypes<TData, TMessage>
    where TData : IResponseDTO where TMessage : IMessage
    {

        TData Data { get; set; }
        bool IsSuccess { get; set; }
        string CustomError { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string _message { get; set; }

        List<string> Errors { get; set; }
        DateTime Timestamp { get; set; }

          IResponseTypes<TData, TMessage> CreateSuccessResponse(
          TData data,
          TMessage message,
          HttpStatusCode statusCode);

        IResponseTypes<TData, TMessage> CreateErrorResponse(
            string errorMessage,
            HttpStatusCode statusCode);

        IResponseTypes<TData, TMessage> CreateErrorResponse(
            TMessage responseMessage,
            HttpStatusCode statusCode,
            List<string> errors = null);

    }
}
