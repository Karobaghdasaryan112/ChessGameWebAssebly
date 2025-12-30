using ChessGame.Core.Services.Constants;
using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.HelperService;
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
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    public class GetOptimizedMoveQueryHandler(
        IValidator<GetOptimizedMoveRequestDTO> validator,
        ILogger<GetOptimizedMoveQueryHandler> logger,
        IBoardService boardService,
        IConnectionService connectionService,
        HelperService helperService,
        IMediator mediator,
        GenericValidationService genericValidationService) :
        MediatR_Base<GetOptimizedMoveRequestDTO, GetOptimizedMoveQueryHandler, IBoardService>(validator, logger,
            boardService),
        IRequestHandler<
            GetOptimizedMoveQuery<
                GetOptimizedMoveRequestDTO,
                ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
    {

        public class SearchResult
        {
            public int Score { get; set; }
            public Position? From { get; set; }
            public Position? To { get; set; }
        }

        public async Task<ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
            Handle(
                GetOptimizedMoveQuery<GetOptimizedMoveRequestDTO,
                    ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>> request,
                CancellationToken cancellationToken)
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


            var aiColor = request.RequestDTO.ChosenColor;
            var depth = HelperConstants.MAX_DEPTH;

            var result = AlphaBetaRoot(request.RequestDTO.GameId, board, depth, int.MinValue, int.MaxValue, aiColor);


            return ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(new GetOptimizedMoveResponseDTO
                {
                    FromPosition = result.From!,
                    ToPosition = result.To!,
                    GameId = request.RequestDTO.GameId
                },
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK);
        }

        private SearchResult AlphaBetaRoot(Guid gameId, Board board, int depth, int alpha, int beta, FigureColors aiColor)
        {
            SearchResult bestResult = new() { Score = int.MinValue };

            var moves = GeneratePossibleMoves(board);

            foreach (var move in moves)
            {
                foreach (var to in move.Value)
                {
                    var nextBoard = (Board)board.Clone();

                    var submit = TryApplyMove(gameId, nextBoard, move.Key, to);
                    if (!submit)
                        continue;

                    nextBoard.SwitchTurn();

                    var score = AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                    if (score > bestResult.Score)
                    {
                        bestResult.Score = score;
                        bestResult.From = move.Key;
                        bestResult.To = to;
                    }

                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha)
                        break;
                }
            }

            return bestResult;
        }


        private int AlphaBeta(Guid gameId, Board board, int depth, int alpha, int beta, FigureColors aiColor)
        {
            if (depth == 0)
                return Evaluate(gameId, board, aiColor);

            bool isMaximizing = (FigureColors)board.Turn == aiColor;

            var moves = GeneratePossibleMoves(board);

            if (isMaximizing)
            {
                int maxEval = int.MinValue;

                foreach (var move in moves)
                {
                    foreach (var to in move.Value)
                    {
                        var nextBoard = (Board)board.Clone();

                        if (!TryApplyMove(gameId, nextBoard, move.Key, to))
                            continue;

                        nextBoard.SwitchTurn();

                        int eval =  AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);

                        if (beta <= alpha)
                            return maxEval;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;

                foreach (var move in moves)
                {
                    foreach (var to in move.Value)
                    {
                        var nextBoard = (Board)board.Clone();

                        if (!TryApplyMove(gameId, nextBoard, move.Key, to))
                            continue;

                        nextBoard.SwitchTurn();

                        int eval =  AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);

                        if (beta <= alpha)
                            return minEval;
                    }
                }
                return minEval;
            }
        }

        private bool TryApplyMove(Guid gameId, Board board, Position from, Position to)
        {
            var cmd = new SubmitMoveCommand<
                SubmitMoveRequestDTO,
                ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                new SubmitMoveRequestDTO
                {
                    GameId = gameId,
                    From = from,
                    To = to,
                    CurrentBoardState = board
                });

            var result = helperService.SubmitMoveByHelper(from, to, board);

            return result is { IsMoveSuccess: true, IsKingChecked: false, IsKingMate: false };
        }

        private int Evaluate(
            Guid gameId,
            Board board,
            FigureColors aiColor)
        {
            int score = 0;

            var blocks = board.BoardBlocks
                .SelectMany(x => x)
                .Where(b => b.Figure != null)
                .ToList();

            score += MaterialScore(blocks, aiColor);
            score += KingSafetyScore(gameId, board, aiColor);

            return score;
        }


        private int MaterialScore(List<Block> blocks, FigureColors aiColor)
        {
            int score = 0;

            foreach (var block in blocks)
            {
                int value = (int)block.Figure.FigureScore;
                score += block.Figure.FigureColor == aiColor ? value : -value;
            }

            return score;
        }
        private int KingSafetyScore(
            Guid gameId,
            Board board,
            FigureColors aiColor)
        {

            var check = helperService.IsKingCheckByHelper(aiColor, board);

            if (!check) return 0;
            var mate = helperService.IsKingMateStateByHelper(board, aiColor);
            if (mate)
                return -100_000;
            return -200;

        }

        private int AttackScore(
            List<Block> blocks,
            Board board,
            FigureColors aiColor)
        {
            int score = 0;

            foreach (var attacker in blocks)
            {
                var moves = attacker.Figure
                    .GetMovableAndCuttableBlocks(attacker.Position, board);

                foreach (var target in moves.CutableBlock)
                {
                    int targetValue = (int)target.Figure.FigureScore;
                    int attackerValue = (int)attacker.Figure.FigureScore;

                    // выгодная атака
                    int attackScore = targetValue - attackerValue / 2;

                    if (attacker.Figure.FigureColor == aiColor)
                        score += Math.Max(0, attackScore);
                    else
                        score -= Math.Max(0, attackScore);
                }
            }

            return score;
        }


        public List<KeyValuePair<Position, List<Position>>> GeneratePossibleMoves(Board board)
        {
            var possibleMoves = new List<KeyValuePair<Position, List<Position>>>();
            foreach (var block in board.BoardBlocks.SelectMany(blocks =>
                         blocks.Where(block =>
                             block.Figure != null && block.Figure?.FigureColor == (FigureColors)board.Turn)))
            {

                var moves = block.Figure.GetMovableAndCuttableBlocks(block.Position, board);

                var movables = moves.MovableBlock.Select(block => block.Position).ToList();
                var cuttables = moves.CutableBlock.Select(block => block.Position).ToList();
                possibleMoves.Add(new KeyValuePair<Position, List<Position>>(block.Position, movables));
                possibleMoves.Add(new KeyValuePair<Position, List<Position>>(block.Position, cuttables));
            }
            return possibleMoves;
        }
    }
}
