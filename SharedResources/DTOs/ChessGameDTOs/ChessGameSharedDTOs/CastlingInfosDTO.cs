using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class CastlingInfosDTO
    {
        public bool IsShortCastle { get; set; }
        public bool IsCastling { get; set; }
        public Position CastlingPosition { get; set; }
    }
}
