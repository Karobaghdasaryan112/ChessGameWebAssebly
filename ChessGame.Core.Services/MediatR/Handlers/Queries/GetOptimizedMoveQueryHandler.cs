using ChessGame.Core.Services.Constants;
using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    public class GetOptimizedMoveQueryHandler(
       IValidator<GetOptimizedMoveRequestDTO> validator,
       ILogger<GetOptimizedMoveQueryHandler> logger,
        IBoardService boardService,
        IMediator mediator,
        GenericValidationService genericValidationService) :
        MediatR_Base<GetOptimizedMoveRequestDTO, GetOptimizedMoveQueryHandler, IBoardService>(validator, logger, boardService),
        IRequestHandler<
            GetOptimizedMoveQuery<
                GetOptimizedMoveRequestDTO,
                ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
            Handle(GetOptimizedMoveQuery<GetOptimizedMoveRequestDTO, ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>> request, CancellationToken cancellationToken)
        {
            //Valdidation
            var validationResult = await genericValidationService.ValidateAsync(request.RequestDTO);
            if (!validationResult.IsValid)
            {
                return await Task.FromResult(ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(ChessGameResponseMessage.InvalidData, System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            //Existing Game Check
            var board = ActiveGames.GetBoard(request.RequestDTO.GameId);
            if (board == null)
            {
                return await Task.FromResult(ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(ChessGameResponseMessage.GameNotFound, System.Net.HttpStatusCode.NotFound));
            }


            //initialize alpha-beta parameters and depth
            int depth = HelperConstants.MAX_DEPTH;
            bool isMaximizingPlayer = true;
            int alpha = HelperConstants.ALPHA;
            int beta = HelperConstants.BETA;

            var currentColor = request.RequestDTO.ChosenColor;//TO DO : Determine current color based on the game state
            try
            {
                var bestScoreResult = await AlphaBeta(request.RequestDTO.GameId, board, depth, alpha, beta, isMaximizingPlayer, currentColor, mediator);
                var bestScoreBoard = _boardAndScores.FirstOrDefault(x => x.Value == bestScoreResult).Key;
                var differenceBlocks = board.CompareTo(bestScoreBoard);
                var bestMove = new GetOptimizedMoveResponseDTO
                {
                    FromPosition = differenceBlocks[1].Position,
                    ToPosition = differenceBlocks[0].Position,
                    GameId = request.RequestDTO.GameId
                };
                return ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                    .CreateSuccessResponse(bestMove, ChessGameResponseMessage.SuccessData, System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var x = 10;
            }
            return null;
        }
        private async Task<int> AlphaBeta(Guid gameId, Board board, int depth, int alpha, int beta, bool isMaximizingPlayer, FigureColors currentColor, IMediator mediator)
        {
            if (depth == 0)
                return await GetStatePoint(gameId, board, currentColor, mediator);

            var possibleMoves = GeneratePossibleMoves(board);

            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;

                foreach (var move in possibleMoves)
                {
                    foreach (var toPosition in move.Value)
                    {

                        var nextBoard = (Board)board.Clone();
                        nextBoard.Turn = board.Turn;

                        var moveCommand = new SubmitMoveCommand<
                            SubmitMoveRequestDTO,
                            ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                            new SubmitMoveRequestDTO
                            {
                                GameId = gameId,
                                From = move.Key,
                                To = toPosition,
                                CurrentBoardState = nextBoard
                            });

                        var result = await mediator.Send(moveCommand);

                        if (result is { IsSuccess: true, Data.IsKingChecked: true } ||
                            result is { IsSuccess: true, Data.IsKingMate: true })
                        {
                            continue;
                        }

                        nextBoard.SwitchTurn();
                        var nextColor = currentColor == FigureColors.White ? FigureColors.Black : FigureColors.White;

                        int eval = await AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, false, nextColor, mediator);

                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);

                        Console.WriteLine($"MaxEval {maxEval}");
                        Console.WriteLine($"ToPosition {toPosition.ToString()}");
                        Console.WriteLine($"nextColor {nextColor}");

                        if (depth == HelperConstants.MAX_DEPTH)
                            _boardAndScores[nextBoard] = eval;

                        if (beta <= alpha)
                            break;
                    }
                }

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;

                foreach (var move in possibleMoves)
                {
                    foreach (var toPosition in move.Value)
                    {
                        var nextBoard = (Board)board.Clone();
                        nextBoard.Turn = board.Turn;

                        var moveCommand = new SubmitMoveCommand<
                            SubmitMoveRequestDTO,
                            ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                            new SubmitMoveRequestDTO
                            {
                                GameId = gameId,
                                From = move.Key,
                                To = toPosition,
                                CurrentBoardState = nextBoard
                            });

                        var result = await mediator.Send(moveCommand);

                        if (result is { IsSuccess: true, Data.IsKingChecked: true } ||
                            result is { IsSuccess: true, Data.IsKingMate: true })
                        {
                            continue;
                        }

                        nextBoard.SwitchTurn();
                        var nextColor = currentColor == FigureColors.White ? FigureColors.Black : FigureColors.White;

                        int eval = await AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, true, nextColor, mediator);

                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);

                        Console.WriteLine($"MaxEval {minEval}");
                        Console.WriteLine($"ToPosition {toPosition.ToString()}");
                        Console.WriteLine($"nextColor {nextColor}");

                        if (depth == HelperConstants.MAX_DEPTH)
                            _boardAndScores[nextBoard] = eval;

                        if (beta <= alpha)
                            break;
                    }
                }

                return minEval;
            }
        }


        private async Task<int> GetStatePoint(Guid gameId, Board board, FigureColors curreentColor, IMediator mediator)
        {
            var blocks = board.BoardBlocks.SelectMany(blocks => blocks.Where(block => block.Figure != null)).ToList();
            int score = 0;

            var isKingMateQuery = new IsKingMateQuery<IsKingMateRequestDTO, ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>(
            new IsKingMateRequestDTO
            {
                ChosenColor = (Turn)curreentColor,
                CurrentBoard = board,
                GameId = gameId
            });

            var isKingCheckQuery = new IsKingCheckedQuery<IsKingCheckedRequestDTO, ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(new IsKingCheckedRequestDTO()
            {
                ChosenColor = (Turn)curreentColor,
                CurrentBoard = board,
                GameId = gameId
            });

            score += MaterialScore(blocks, curreentColor);
            score += AttackScore(blocks, board, curreentColor);
            score += await KingSafetyScore(board, curreentColor, mediator, isKingMateQuery, isKingCheckQuery);

            return score;
        }
        int MaterialScore(List<Block> blocks, FigureColors currentColor)
        {
            int score = 0;

            foreach (var block in blocks)
            {
                int value = (int)block.Figure.FigureScore;

                score += block.Figure.FigureColor == currentColor ? value : -value;
            }

            return score;
        }
        int AttackScore(List<Block> blocks, Board board, FigureColors currentColor)
        {
            int score = 0;

            foreach (var block in blocks)
            {
                var cuttableBlocks = block.Figure.GetMovableAndCuttableBlocks(block.Position, board).CutableBlock;
                foreach (var cuttableBlock in cuttableBlocks)
                {
                    var value = (int)cuttableBlock.Figure.FigureScore / 10;

                    score += cuttableBlock.Figure.FigureColor == currentColor
                        ? value
                        : -value;
                }
            }
            return score;
        }


        async Task<int> KingSafetyScore(
            Board board,
            FigureColors currentColor,
            IMediator mediator,
            IsKingMateQuery<IsKingMateRequestDTO, ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>> isKingMateQuery,
            IsKingCheckedQuery<IsKingCheckedRequestDTO, ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>> isKingCheckedQuery)
        {
            var isKingMateResponse = await mediator.Send(isKingMateQuery);
            if (isKingMateResponse is { IsSuccess: true, Data.IsKingMate: true })
            {

                return (FigureColors)board.Turn == currentColor
                    ? -100_000
                    : 100_000;
            }

            var isKingCheckedResponse = await mediator.Send(isKingCheckedQuery);
            if (isKingCheckedResponse is { IsSuccess: true, Data.IsKingChecked: true })
            {
                return (FigureColors)board.Turn == currentColor
                    ? -200
                    : 200;
            }

            return 0;
        }
        public List<KeyValuePair<Position, List<Position>>> GeneratePossibleMoves(Board board)
        {
            var possibleMoves = new List<KeyValuePair<Position, List<Position>>>();

            foreach (var block in board.BoardBlocks.SelectMany(blocks => blocks.Where(block => block.Figure != null && block.Figure?.FigureColor == (FigureColors)board.Turn)))
            {
                var moves = block.Figure.GetMovableAndCuttableBlocks(block.Position, board);
                var movables = moves.MovableBlock.Select(block => block.Position).ToList();
                possibleMoves.Add(new KeyValuePair<Position, List<Position>>(block.Position, movables));
            }
            return possibleMoves;
        }
        private Dictionary<Board, int> _boardAndScores { get; set; } = new Dictionary<Board, int>();

    }
}
