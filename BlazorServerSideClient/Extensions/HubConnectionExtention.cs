using BlazorServerSideClient.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;
using System.Net;


namespace BlazorServerSideClient.Extensions
{
    public static class HubConnectionExtension
    {
        public static async Task<PipeLineResponse<TResponse>?> SafeInvokeAsync<TRequest, TResponse>(
            this HubConnection hubConnection,
            string identifier,
            TRequest request,
            JSRunetimeService runeTimeService)
        {
            var pipeLineResponse = new PipeLineResponse<TResponse>();
            try
            {
                var response = await hubConnection
                    .InvokeAsync<PipeLineResponse<TResponse>>(identifier, request);

                if (response.Response?.IsSuccess == true)
                    return response;

                var errorMessage =
                    response.Response?.Errors?.Any() == true
                        ? string.Join(" ", response.Response.Errors)
                        : !string.IsNullOrWhiteSpace(response.Response?.CustomError)
                            ? response.Response.CustomError
                            : response.Response?.Message?.MessageOutput
                              ?? "Unknown error";

                await runeTimeService.ShowErrorModal(errorMessage);

                return response;
            }
            catch (Exception ex)
            {
                await runeTimeService.ShowErrorModal(ex.Message);

                pipeLineResponse.Response =
                (
                    new ResponseDTO<TResponse, ChessGameResponseMessage>()
                ).CreateErrorResponse(ex.Message, HttpStatusCode.InternalServerError);

                return pipeLineResponse;
            }
        }
    }
}
