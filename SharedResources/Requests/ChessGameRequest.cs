using SharedResources.Contracts;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class ChessGameRequest<T> : IRequestTypes<T> where T : IRequestDTO
    {
        public T requestType { get ; set ; }

        public ChessGameRequest(T requestType)
        {
            this.requestType = requestType;
        }
    }
}
