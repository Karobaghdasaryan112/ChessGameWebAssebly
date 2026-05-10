using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SharedResources.ChessGameResource.Enums.Training;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    // DTO for requesting a training game
    //Computer Player Guid is Guid.Empty
    public class TrainingGameRequestDTO : RequestDTO
    {
        [JsonPropertyName("trainingDifficulty")]
        public TrainingDifficulty TrainingDifficulty { get; set; }

        [JsonPropertyName("player1Name")]
        public string Player1Name { get; set; }

        [JsonPropertyName("player2Name")]
        public string Player2Name { get; set; }

        [JsonPropertyName("player1Guid")]
        public Guid Player1Guid { get; set; }

        [JsonPropertyName("player2Guid")]
        public Guid Player2Guid { get; set; }

        public TrainingGameRequestDTO() { }
    }
}