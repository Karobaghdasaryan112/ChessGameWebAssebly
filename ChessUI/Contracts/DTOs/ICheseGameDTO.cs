namespace SharedResources.Contracts.DTOs
{
    public interface ICheseGameResponseDTO : IResponseDTO
    {
        public int GameId { get; set; }
    }

    public interface ICheseGameRequestDTO : IRequestDTO
    {
        public int GameId { get; set; }
    }
}
