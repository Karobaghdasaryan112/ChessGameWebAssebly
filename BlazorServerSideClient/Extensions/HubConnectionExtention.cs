using BlazorServerSideClient.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using BlazorServerSideClient.Helpers;


namespace BlazorServerSideClient.Extensions
{
    public static class HubConnectionExtension
    {
        public static async Task<PipeLineResponse<TResponse>?> SafeInvokeAsync<TRequest, TResponse>(
            this HubConnection hubConnection,
            string identifier,
            TRequest request,
            JSRunetimeService runeTimeService)
            where TRequest : RequestDTO
        {
            var pipeLineResponse = new PipeLineResponse<TResponse>();
            var pipeLineRequest = new PipeLineRequest<TRequest> { Request = request };
            try
            {
                var response = await hubConnection
                    .InvokeAsync<PipeLineResponse<TResponse>>(identifier, pipeLineRequest);

                if (response.Response?.IsSuccess == true)
                    return response;

                await runeTimeService.ShowErrorModal(ErrorMessageBuilder.BuildErrorMessage(response));

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