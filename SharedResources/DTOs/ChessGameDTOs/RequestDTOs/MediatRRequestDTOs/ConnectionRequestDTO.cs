using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs
{
    public class ConnectionRequestDTO<TDto>
    {
        public TDto Data { get; set; }
    }
}
