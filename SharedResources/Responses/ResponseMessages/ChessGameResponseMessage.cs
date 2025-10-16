using SharedResources.Contracts.RequestsAndResponses;
using System.Text.Json.Serialization;

namespace SharedResources.Responses.ResponseMessages
{
    public class ChessGameResponseMessage : IMessage
    {
        public string MessageOutput { get; }
        public ChessGameResponseMessage(string messageOutput)
        {
            MessageOutput = messageOutput;
        }

        [JsonConstructor]
        public ChessGameResponseMessage()
        {

        }

        public static readonly ChessGameResponseMessage GameCreated =
            new ChessGameResponseMessage("Chess game created successfully.");

        public static readonly ChessGameResponseMessage GameCreationFailed =
            new ChessGameResponseMessage("Failed to create chess game.");

        public static readonly ChessGameResponseMessage GameNotFound =
            new ChessGameResponseMessage("Chess game not found.");

        public static readonly ChessGameResponseMessage MoveSuccessful =
            new ChessGameResponseMessage("Move completed successfully.");

        public static readonly ChessGameResponseMessage InvalidMove =
            new ChessGameResponseMessage("Invalid move.");

        public static readonly ChessGameResponseMessage PlayerNotFound =
            new ChessGameResponseMessage("Player not found.");

        public static readonly ChessGameResponseMessage GameOver =
            new ChessGameResponseMessage("Game over.");

        public static readonly ChessGameResponseMessage PlayerWon =
            new ChessGameResponseMessage("Player won the game.");

        public static readonly ChessGameResponseMessage Draw =
            new ChessGameResponseMessage("The game ended in a draw.");

        public static readonly ChessGameResponseMessage InternalServerError =
            new ChessGameResponseMessage("Internal server error. Please try again later.");
    }
}
