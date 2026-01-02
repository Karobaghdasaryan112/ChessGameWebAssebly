using SharedResources.ChessGameResource.Enums.Training;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    // DTO for requesting a training game
    //Computer Player Guid is Guid.Empty
    public class TrainingGameRequestDTO
    {
        public string ClientConnectionId { get; set; }
        public TrainingDifficulty TrainingDifficulty { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public Guid Player1Guid { get; set; }
        public Guid Player2Guid { get; set; }

    }
}
