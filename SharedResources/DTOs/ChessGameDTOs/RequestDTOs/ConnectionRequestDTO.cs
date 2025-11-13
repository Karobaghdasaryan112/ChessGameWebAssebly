using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class ConnectionRequestDTO<TDto> 
    {
        public TDto Data;
    }
}
