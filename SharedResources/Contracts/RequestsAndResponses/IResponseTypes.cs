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
        TMessage message { get; set; }

        List<string> Errors { get; set; }
        DateTime Timestamp { get; set; }
    }
}

