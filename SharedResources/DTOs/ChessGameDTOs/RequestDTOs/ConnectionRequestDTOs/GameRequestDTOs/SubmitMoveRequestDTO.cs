using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SubmitMoveRequestDTO : RequestDTO
    {
        public FigureType PromotionFigure { get; set; }
        public Guid GameId { get; set; }
        public Position? From { get; set; }
        public Position? To { get; set; }
        public Board CurrentBoardState { get; set; }
    }
}
