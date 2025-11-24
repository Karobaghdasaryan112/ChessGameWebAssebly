using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;
namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService<THub> : IGameService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IConnectionService<THub> _connectionService;
        private readonly IBoardService _boardService;
        private BaseHubService<THub> _baseHubService;

        public GameService(IConnectionService<THub> connectionService, IBoardService boardService, BaseHubService<THub> baseHubService)
        {
            _boardService = boardService;
            _connectionService = connectionService;
            _baseHubService = baseHubService;
        }

        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
            var onlinePlayers = _connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != connectionRequestDTO.Data.UserGuid)
                .ToDictionary();
            if (onlinePlayers.Count() == 0)
                return
                    ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(
                    default,
                    ChessGameResponseMessage.UserConnectionNotFound,
                    System.Net.HttpStatusCode.BadRequest);
            return
                ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new GetOnlinePlayersResponseDTO() { OnlinePlayers = onlinePlayers },
                    ChessGameResponseMessage.UserConnectionFoundSuccess,
                    System.Net.HttpStatusCode.OK);
        }


        public async Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO)
        {
            var games = ActiveGames.ActiveGamesAndBoards;
            var gameState = ActiveGames.ActiveGamesAndBoards.Where(kvp => kvp.Key == gameStateReqeustDTO.Data.GameId).FirstOrDefault();
            return await Task.Run(() => new ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SendGameStateResponseDTO()
                {
                    Board = gameState.Value
                },
                Message = ChessGameResponseMessage.GameCreated,
            });
        }
        public async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            //current Board State from Server
            var gameState =
                ActiveGames.ActiveGamesAndBoards.
                Where(kvp =>
                kvp.Key == sendMoveConnectionRequestDTO.Data.GameId).
                First().Value;

            gameState.FigureColor = sendMoveConnectionRequestDTO.Data.MyColor;

            //if the request is movable or cutable then return the block from the Active Games (current Board State from Server)
            var canMoveResultBlock = await _boardService.CanClick(sendMoveConnectionRequestDTO.Data.MyColor, sendMoveConnectionRequestDTO.Data.CurrentBlock, sendMoveConnectionRequestDTO.Data.PreviusBlockInformationDTO, gameState);

            if (canMoveResultBlock == default)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                   new MoveResponseDTO()
                   {
                       GameId = sendMoveConnectionRequestDTO.Data.GameId,
                       Player = sendMoveConnectionRequestDTO.Data.Player
                   },
                   ChessGameResponseMessage.InvalidMove,
                   System.Net.HttpStatusCode.BadRequest);



            //if to Position is null that means the figure is Clicked for Move 
            if (sendMoveConnectionRequestDTO.Data.To == null)
            {
                var positions =
                gameState.
                GetBlockByPosition(sendMoveConnectionRequestDTO.Data.From).
                Figure.
                GetMovableAndCutableBlocks(sendMoveConnectionRequestDTO.Data.From, gameState);

                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new MoveResponseDTO()
                    {
                        CutableBlocks = positions.CutableBlock,
                        MovableBlocks = positions.MovableBlock,
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,
                        Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.SuccessUserConnections,
                    System.Net.HttpStatusCode.OK);
            }
            //



            //if there is To position then its clicked For Move or Cut (if event is not cutable or movable then return error response)
            if (canMoveResultBlock.EventColor != SharedResources.ChessGameResource.Enums.Colors.EventColors.Cut &&
                canMoveResultBlock.EventColor != SharedResources.ChessGameResource.Enums.Colors.EventColors.Move)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                                 new MoveResponseDTO()
                                 {
                                     GameId = sendMoveConnectionRequestDTO.Data.GameId,
                                     Player = sendMoveConnectionRequestDTO.Data.Player
                                 },
                                 ChessGameResponseMessage.InvalidMove,
                                 System.Net.HttpStatusCode.BadRequest);





            //send to second client from the group GameId for Updating after Move
            //find secondClinet ConnectionId

            var selectedGameKeyValue = _connectionService.CurrentConnectionState.
             Where(gameId_UserConnection =>
                 gameId_UserConnection.Value?.GameId ==
                 sendMoveConnectionRequestDTO.Data.GameId &&
                 sendMoveConnectionRequestDTO.Data.Player != gameId_UserConnection.Value?.UserName).
             Select(selectedGame_UserConnection => selectedGame_UserConnection.Value).ToList();

            if (selectedGameKeyValue == null)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new MoveResponseDTO()
                    {
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,
                        Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest); 

            var selectedGameOpponentConnectionId = selectedGameKeyValue.First().ConnectionId;


            var boardStateResposneDTO = new BoardStateResponseDTO()
            {   
                GameId = sendMoveConnectionRequestDTO.Data.GameId,
                CutableFigure = default,
                From = sendMoveConnectionRequestDTO.Data.From,
                To = sendMoveConnectionRequestDTO.Data.To,
                OpponentConnectionId = selectedGameOpponentConnectionId,
                OpponentColor =
                    sendMoveConnectionRequestDTO.Data.MyColor == FigureColors.Black ?
                    FigureColors.White :
                    FigureColors.Black,
            };



            //If this is Movable condition 
            if (canMoveResultBlock.EventColor == SharedResources.ChessGameResource.Enums.Colors.EventColors.Move)
            {

                var sendBoardResposneDTO = ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(boardStateResposneDTO, ChessGameResponseMessage.Draw);

                //baseHubService call for opponent Client

                    await _baseHubService.ReceiveBoardUpdateAsync(sendBoardResposneDTO);


                //submit the move for my Client 
                var moveResult = await _boardService.SubmitMoveAsync(sendMoveConnectionRequestDTO.Data.GameId, sendMoveConnectionRequestDTO.Data.From, sendMoveConnectionRequestDTO.Data.To, gameState);

                if (moveResult)
                    return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new MoveResponseDTO()
                        {
                            GameId = sendMoveConnectionRequestDTO.Data.GameId,
                            Player = sendMoveConnectionRequestDTO.Data.Player
                        },
                        ChessGameResponseMessage.MoveSuccessful,
                        System.Net.HttpStatusCode.OK);

                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new MoveResponseDTO()
                    {
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,
                        Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);
            }

            //Cutable Condition
            //TO DO: send to second client from the group GameId for Updating after Cut
            if (canMoveResultBlock.EventColor == SharedResources.ChessGameResource.Enums.Colors.EventColors.Cut)
            {

            }
            return default;
        }
    }
}
