using SharedResources.Contracts.RequestsAndResponses;
using System.Text.Json.Serialization;

namespace SharedResources.Responses.ResponseMessages
{
    public class ChessGameResponseMessage : IMessage
    {
        public string MessageOutput { get; set; }
        public ChessGameResponseMessage(string messageOutput)
        {
            MessageOutput = messageOutput;
        }

        [JsonConstructor]
        public ChessGameResponseMessage()
        {

        }
        public static readonly ChessGameResponseMessage SuccessUserConnections =
            new ChessGameResponseMessage("Get User Connections Success.");

        public static readonly ChessGameResponseMessage UsersRemovedFromGameSuccess =
            new ChessGameResponseMessage("Users Removed Success.");

        public static readonly ChessGameResponseMessage InvalidData =
            new ChessGameResponseMessage("Invalid game data received.");  
        
        public static readonly ChessGameResponseMessage SuccessData =
            new ChessGameResponseMessage("Data is Success");

        public static readonly ChessGameResponseMessage UserConnectionNotFound =
            new ChessGameResponseMessage("User Connection Is Not Found.");

        public static readonly ChessGameResponseMessage UserConnectionFoundSuccess =
            new ChessGameResponseMessage("User Connection Is Found SuccessFully.");

        public static readonly ChessGameResponseMessage SuccessInvitation =
             new ChessGameResponseMessage("Invitation Create Success.");

        public static readonly ChessGameResponseMessage UserConnectionRemovedSuccess =
            new ChessGameResponseMessage("User Connection Is Removed Success.");

        public static readonly ChessGameResponseMessage ConnectionAddedSuccess =
            new ChessGameResponseMessage("User Connection Is Added Success.");

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
