using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Requests
{
    public class IdentityRequest : IRequestTypes<IIdentityDTO>
    {
        public IIdentityDTO requestType { get; set; }
        public IdentityRequest(IIdentityDTO requestType)
        {
            this.requestType = requestType;
        }
    }
}
