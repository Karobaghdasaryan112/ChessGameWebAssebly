using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class CanClickRequestDTO
    {
        public FigureColors FigureColor;
        public Block? CurrentBlock;
        public ClickedBlockInformationDTO? ClickedBlockInformationDto;
        public Board? CurrentBoardBoardState;
    }
}
