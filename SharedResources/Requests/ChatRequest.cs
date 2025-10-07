using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class ChatRequest : IRequestTypes<IChatDTO>
    {
        public IChatDTO requestType { get; set; }
        public ChatRequest(IChatDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
