using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class IChessGameRequest : IRequestTypes<ICheseGameDTO>
    {
        public ICheseGameDTO requestType { get; set; }
        public IChessGameRequest(ICheseGameDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
