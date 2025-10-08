using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class IdentityRequest : IRequestTypes<IIdentityRequestDTO>
    {
        public IIdentityRequestDTO requestType { get; set; }
        public IdentityRequest(IIdentityRequestDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
