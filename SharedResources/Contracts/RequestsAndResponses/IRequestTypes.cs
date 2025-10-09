namespace SharedResources.Contracts.RequestsAndResponses
{
    public interface IRequestTypes<TRequestType> where TRequestType : IRequestDTO
    {
        TRequestType requestType { get; set; }
    }
}
