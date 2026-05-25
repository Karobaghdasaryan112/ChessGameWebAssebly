using System.Text.Json.Serialization;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs
{
    public class RequestDTO
    {
        [JsonIgnore]
        public string connectionId { get; set; }
    }
}
