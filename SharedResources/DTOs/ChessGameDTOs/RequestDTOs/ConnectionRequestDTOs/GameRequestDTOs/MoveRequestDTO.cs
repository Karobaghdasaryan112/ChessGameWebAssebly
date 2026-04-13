using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class MoveRequestDTO : RequestDTO
    {
        public bool IsAIFirstMove { get; set; }
        public bool IsOpponentComputer { get; set; }
        public string Player { get; set; }
        public Guid CurrentPlayerId { get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
        public Position CurrentPosition { get; set; }
        public ClickedBlockInformationDTO PreviusBlockInformationDTO { get; set; }
        public FigureColors MyColor { get; set; }
        public Guid GameId { get; set; }
    }
}
