namespace SharedResources.Contracts.DTOs
{
    public interface ICheseGameResponseDTO : IResponseDTO
    {
        public Guid GameId { get; set; }
    }

    public interface ICheseGameRequestDTO : IRequestDTO
    {
        public Guid GameId { get; set; }
    }
}
