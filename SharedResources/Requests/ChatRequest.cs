using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class ChatRequest : IRequestTypes<IChatRequestDTO>
    {
        public IChatRequestDTO requestType { get; set; }
        public ChatRequest(IChatRequestDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
