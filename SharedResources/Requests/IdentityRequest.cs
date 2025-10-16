using SharedResources.Contracts;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class IdentityRequest<T> : IRequestTypes<T> where T : IRequestDTO
    {
        public T requestType { get; set; }

        public IdentityRequest(T requestType)
        {
            this.requestType = requestType;
        }
    }
}
