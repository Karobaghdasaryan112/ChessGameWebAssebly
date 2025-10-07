namespace SharedResources.Contracts.RequestsAndResponses
{
    public interface IRequestTypes<TRequestType> where TRequestType : class
    {
        TRequestType requestType { get; set; }
    }
}
