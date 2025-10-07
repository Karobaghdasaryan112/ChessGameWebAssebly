using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Responses.ResponseMessages
{
    public class ChessGameResponseMessage : IMessage
    {
        public string GameCreated => "Chess game created successfully.";
        public string GameCreationFailed => "Failed to create chess game.";
        public string GameNotFound => "Chess game not found.";
        public string MoveSuccessful => "Move completed successfully.";
        public string InvalidMove => "Invalid move.";
        public string PlayerNotFound => "Player not found.";
        public string GameOver => "Game over.";
        public string PlayerWon => "Player won the game.";
        public string Draw => "The game ended in a draw.";
        public string InternalServerError => "Internal server error. Please try again later.";
    }
}
