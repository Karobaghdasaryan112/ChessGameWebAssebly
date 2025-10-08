using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class ChessGameRequest : IRequestTypes<ICheseGameRequestDTO>
    {
        public ICheseGameRequestDTO requestType { get; set; }
        public ChessGameRequest(ICheseGameRequestDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
