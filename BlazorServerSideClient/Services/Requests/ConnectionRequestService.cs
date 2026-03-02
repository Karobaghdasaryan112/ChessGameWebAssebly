using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class ConnectionRequestService(SignalRService signalRService, JSRunetimeService _jsRunetimeService) : IConnectionReqeustService
    {

        public Task<
            ResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> GetUserConnection(GetUserConnectionRequestDTO getUserConnectionRequestDTO)
             => _jsRunetimeService.SendAsync<
                    GetUserConnectionRequestDTO,
                    ResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>("GetUserConnection", getUserConnectionRequestDTO);

        public Task<
            ResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO)
             => _jsRunetimeService.SendAsync<
                    AddUserConnectionRequestDTO,
                    ResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>("AddConnectionAsync", addUserConnectionRequestDTO);

        public Task<
            ResponseDTO<
                RemoveUserConnectionResponseDTO,
                ChessGameResponseMessage>> RemoveConnectionAsync(
            RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
             => _jsRunetimeService.SendAsync<
                    RemoveUserConnectionRequestDTO,
                    ResponseDTO<
                        RemoveUserConnectionResponseDTO,
                        ChessGameResponseMessage>>("RemoveConnectionAsync", removeUserConnectionRequestDTO);

    }
}