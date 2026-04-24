using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.PipeLine.PipeLineContext;

public class PipeLineRequest<TRequest> where TRequest : RequestDTO
{
    public PipeLineRequest()
    {
        
    }
    public PipeLineRequest(TRequest request)
    {
        Request = request;
    }
    public TRequest Request { get; set; }
}