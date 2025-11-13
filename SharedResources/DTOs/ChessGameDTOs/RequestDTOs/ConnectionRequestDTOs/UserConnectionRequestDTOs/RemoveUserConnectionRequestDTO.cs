namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class RemoveUserConnectionRequestDTO
    {
        //this two fields for 2 methods 
        public Guid UserGuid { get; set; }
        public string ConnectionId { get; set; }
    }
}
