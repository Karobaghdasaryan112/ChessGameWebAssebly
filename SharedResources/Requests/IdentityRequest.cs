using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    //public class IdentityRequest<TDto> : IRequestTypes<IIdentityRequestDTO>
    //{
    //    public IIdentityRequestDTO requestType { get; set; }
    //    public IdentityRequest(IIdentityRequestDTO requestType)
    //    {
    //        this.requestType = requestType;
    //    }
    //}
    public class IdentityRequest<T> : IRequestTypes<T>
    {
        public T requestType { get; set; }

        public IdentityRequest(T requestType)
        {
            this.requestType = requestType;
        }
    }
}
