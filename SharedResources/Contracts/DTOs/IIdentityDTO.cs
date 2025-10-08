namespace SharedResources.Contracts.DTOs
{
    public interface IIdentityRequestDTO
    {
        public int UserId { get; set; }
    }
    public interface IIdentityResponseDTO : IResponseDTO
    {
        public int UserId { get; set; }
    }
}
