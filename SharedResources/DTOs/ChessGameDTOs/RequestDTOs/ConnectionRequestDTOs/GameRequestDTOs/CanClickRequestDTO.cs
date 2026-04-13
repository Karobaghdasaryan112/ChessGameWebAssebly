using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class CanClickRequestDTO : RequestDTO
    {
        public FigureColors FigureColor;
        public Block? CurrentBlock;
        public ClickedBlockInformationDTO? ClickedBlockInformationDto;
        public Board? CurrentBoardBoardState;
    }
}
