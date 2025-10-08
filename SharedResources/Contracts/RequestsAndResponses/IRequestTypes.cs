namespace SharedResources.Contracts.RequestsAndResponses
{
    public interface IRequestTypes<TRequestType> 
    {
        TRequestType requestType { get; set; }
    }
}
