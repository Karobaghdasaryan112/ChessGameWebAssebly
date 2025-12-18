using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetGameHistoryResponseDTO : IResponseDTO
    {
        public List<string> FEN { get; set; } = [];
        public Guid GameId { get; set; }
        public Guid Player1Guid { get; set; }
        public Guid Player2Guid { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public string Player2Name { get; set; } = string.Empty;
    }
}
