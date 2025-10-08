using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Requests;
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
        public string _message { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static ChessGameResponse<TDto> _chessGameResponse { get => new ChessGameResponse<TDto>(); }

        public IResponseTypes<TDto, ChessGameResponseMessage> CreateErrorResponse(
            string errorMessage,
            HttpStatusCode statusCode)
        {
            _chessGameResponse.Data = default;
            _chessGameResponse.IsSuccess = false;
            _chessGameResponse.CustomError = errorMessage;
            _chessGameResponse.StatusCode = statusCode;
            return _chessGameResponse;
        }

        public IResponseTypes<TDto, ChessGameResponseMessage> CreateErrorResponse(
            ChessGameResponseMessage responseMessage,
            HttpStatusCode statusCode,
            List<string> errors = null)
        {
            _chessGameResponse.Data = default;
            _chessGameResponse.IsSuccess = false;
            _chessGameResponse.Message = responseMessage;
            _chessGameResponse.StatusCode = statusCode;
            _chessGameResponse.Errors = errors ?? new List<string>();
            return _chessGameResponse;
        }

        public IResponseTypes<TDto, ChessGameResponseMessage> CreateSuccessResponse(
            TDto data,
            ChessGameResponseMessage message,
            HttpStatusCode statusCode)
        {
            _chessGameResponse.Data = data;
            _chessGameResponse.IsSuccess = true;
            _chessGameResponse.Message = message;
            _chessGameResponse.StatusCode = statusCode;
            return _chessGameResponse;
        }

    }
}
