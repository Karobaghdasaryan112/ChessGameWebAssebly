namespace ChessGame.Core.Services.PipeLine;

public class PipeLineResponse<TResponse>(TResponse response,bool isSuccess,string errorMessage)
{

    public TResponse Response { get; } = response;
    public bool IsSuccess { get; set; } = isSuccess;
    public string ErrorMessage { get; set; } = errorMessage;
    
}