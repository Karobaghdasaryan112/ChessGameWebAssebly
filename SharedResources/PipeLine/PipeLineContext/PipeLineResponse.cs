using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.PipeLineContext;

public class PipeLineResponse<TResponse>
{
    public ResponseDTO<TResponse, ChessGameResponseMessage> Response { get; set; }

}