using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.PipeLineContext;

public class PipeLineResponse<TResponse, TMessage> 
    where TMessage : ChessGameResponseMessage 
{
    public ResponseDTO<TResponse, TMessage> Response { get; set; }

}