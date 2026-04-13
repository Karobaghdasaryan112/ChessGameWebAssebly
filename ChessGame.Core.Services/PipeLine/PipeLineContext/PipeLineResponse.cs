using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.PipeLine;

public class PipeLineResponse<TResponse, TMessage> 
    where TMessage : ChessGameResponseMessage 
{
    public ResponseDTO<TResponse, TMessage> Response { get; set; }

}