namespace BlazorServerSideClient.Contracts
{
    public interface IHubMessageSender
    {
        Task SendMessageAsync(string methodName, params object[] args);

    }
}
