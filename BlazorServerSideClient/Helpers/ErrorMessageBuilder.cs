using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Helpers;

public class ErrorMessageBuilder
{
    public static string BuildErrorMessage<TResponse>(PipeLineResponse<TResponse> response)
        =>
            response.Response?.Message != null
                ? response.Response.Message.MessageOutput
                : response.Response?.Errors?.Any() == true
                    ? string.Join(" ", response.Response.Errors)
                    : !string.IsNullOrWhiteSpace(response.Response?.CustomError)
                        ? response.Response.CustomError
                        : "Something went wrong!";
}