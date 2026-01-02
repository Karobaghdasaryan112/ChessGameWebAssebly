using FluentValidation;
using FluentValidation.Results;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SubmitMoveResponseDTO = SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO;

namespace ChessGame.Core.Services.Services.Validations
{
    public class GenericValidationService(
        //Game
        //Requests
        IValidator<TrainingGameRequestDTO> trainingGameRequestValidator,
        IValidator<BoardInitializeRequestDTO> boardInitializeRequestValidator,
        IValidator<BoardStateRequestDTO> boardStateRequestValidator,
        IValidator<CanClickRequestDTO> canClickRequestValidator,
        IValidator<ClickRequestDTO> clickRequestValidator,
        IValidator<GetONlinePlayersRequestDTO> getOnlinePlayersValidator,
        IValidator<MoveRequestDTO> moveRequestValidator,
        IValidator<SendGameStateReqeustDTO> sendGameStateRequestValidator,
        IValidator<SubmitMoveRequestDTO> submitMoveRequestValidator,
        IValidator<IsKingMateRequestDTO> iskingMateStateRequestValidator,
        IValidator<GetGameHistoryRequestDTO> getGameHistoryRequestValidator,
        IValidator<GetOptimizedMoveRequestDTO> getOptimizedMoveRequestValidator,

        //Game Responses
        IValidator<BoardStateResponseDTO> boardStateResponseValidator,
        IValidator<CanClickResponseDTO> canClickResponseValidator,
        IValidator<ClickResponseDTO> clickResponseValidator,
        IValidator<GetOnlinePlayersResponseDTO> getOnlinePlayersResponseValidator,
        IValidator<MoveResponseDTO> moveResponseValidator,
        IValidator<SubmitMoveResponseDTO> submitMoveResponseValidator,
        IValidator<ReceivePlayersResponseDTO> receivePlayersResponseValidator,
        IValidator<SendGameStateResponseDTO> sendGameStateResponseValidator,


        //Connection
        //Requests
        IValidator<AddUserConnectionRequestDTO> addUserConnectionValidator,
        IValidator<GetUserConnectionRequestDTO> getUserConnectionValidator,
        IValidator<RemoveUserConnectionRequestDTO> removeUserConnectionValidator,
        IValidator<RemoveUserFromGameRequestDTO> removeUserFromGameRequestValidator,

        //Connection Responses
        IValidator<AddUserConnectionResponseDTO> addUserConnectionResponseValidator,
        IValidator<GetUserConnectionResponseDTO> getUserConnectionResponseValidator,
        IValidator<RemoveUserConnectionResponseDTO> removeUserConnectionResponseValidator,


        //Invitation
        //Requests
        IValidator<AcceptInvitationRequestDTO> acceptInvitationRequestValidator,
        IValidator<SendInvitationRequestDTO> sendInvitationRequestValidator,

        //Responses
        IValidator<AcceptInvitationResponseDTO> acceptInvitationResponseValidator,
        IValidator<SendInvitationsResponseDTO> sendInvitationResponseValidator,

        //Widgets
        //Requests
        IValidator<GetAllHistoryWidgetRequestDTO> getAllHistoryWidgetsRequestValidator,
        IValidator<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO> getGamesByCurrentAndOpponentIdsPaginationRequestDTOValidator
        )

    {
        public async Task<ValidationResult> ValidateAsync<T>(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            IValidator<T> validator = dto switch
            {
                //////////////////////////////////////////////////////////////////////////////////////
                //Game
                // Requests
                TrainingGameRequestDTO => (IValidator<T>)trainingGameRequestValidator,
                BoardInitializeRequestDTO => (IValidator<T>)boardInitializeRequestValidator,
                BoardStateRequestDTO => (IValidator<T>)boardStateRequestValidator,
                CanClickRequestDTO => (IValidator<T>)canClickRequestValidator,
                ClickRequestDTO => (IValidator<T>)clickRequestValidator,
                GetONlinePlayersRequestDTO => (IValidator<T>)getOnlinePlayersValidator,
                MoveRequestDTO => (IValidator<T>)moveRequestValidator,
                SendGameStateReqeustDTO => (IValidator<T>)sendGameStateRequestValidator,
                SubmitMoveRequestDTO => (IValidator<T>)submitMoveRequestValidator,
                IsKingMateRequestDTO => (IValidator<T>)iskingMateStateRequestValidator,
                GetGameHistoryRequestDTO => (IValidator<T>)getGameHistoryRequestValidator,
                GetOptimizedMoveRequestDTO => (IValidator<T>)getOptimizedMoveRequestValidator,


                // Responses
                BoardStateResponseDTO => (IValidator<T>)boardStateResponseValidator,
                CanClickResponseDTO => (IValidator<T>)canClickResponseValidator,
                ClickResponseDTO => (IValidator<T>)clickResponseValidator,
                GetOnlinePlayersResponseDTO => (IValidator<T>)getOnlinePlayersResponseValidator,
                MoveResponseDTO => (IValidator<T>)moveResponseValidator,
                SubmitMoveResponseDTO => (IValidator<T>)submitMoveResponseValidator,
                ReceivePlayersResponseDTO => (IValidator<T>)receivePlayersResponseValidator,
                SendGameStateResponseDTO => (IValidator<T>)sendGameStateResponseValidator,

                //////////////////////////////////////////////////////////////////////////////////////

                //Connection
                // Requests
                AddUserConnectionRequestDTO => (IValidator<T>)addUserConnectionValidator,
                GetUserConnectionRequestDTO => (IValidator<T>)getUserConnectionValidator,
                RemoveUserConnectionRequestDTO => (IValidator<T>)removeUserConnectionValidator,
                RemoveUserFromGameRequestDTO => (IValidator<T>)removeUserFromGameRequestValidator,

                // Responses
                GetUserConnectionResponseDTO => (IValidator<T>)getUserConnectionResponseValidator,
                AddUserConnectionResponseDTO => (IValidator<T>)addUserConnectionResponseValidator,
                RemoveUserConnectionResponseDTO => (IValidator<T>)removeUserConnectionResponseValidator,

                //////////////////////////////////////////////////////////////////////////////////////

                //Invitation
                // Requests
                AcceptInvitationRequestDTO => (IValidator<T>)acceptInvitationRequestValidator,
                SendInvitationRequestDTO => (IValidator<T>)sendInvitationRequestValidator,

                // Responses
                AcceptInvitationResponseDTO => (IValidator<T>)acceptInvitationResponseValidator,
                SendInvitationsResponseDTO => (IValidator<T>)sendInvitationResponseValidator,

                //////////////////////////////////////////////////////////////////////////////////////

                //Widgets
                //Requests
                GetAllHistoryWidgetRequestDTO => (IValidator<T>)getAllHistoryWidgetsRequestValidator,
                GetGamesByCurrentAndOpponentIdsPaginationRequestDTO => (IValidator<T>)getGamesByCurrentAndOpponentIdsPaginationRequestDTOValidator,

                _ => throw new InvalidOperationException($"No validator registered for type {typeof(T).Name}")
            };

            return await validator.ValidateAsync(dto);
        }

        public async Task ValidateAndThrowAsync<T>(T dto)
        {
            var result = await ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Validation failed for {typeof(T).Name}: {errors}");
            }
        }

    }

    public static class ValidationExtension
    {
        public static async Task<ResponseDTO<TDto, ChessGameResponseMessage>> ReturnValidationResult<TDto>(this ValidationResult validationResult, TDto dto)
        {
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                return await
                    Task.FromResult(
                        ResponseDTO<TDto, ChessGameResponseMessage>
                        .CreateErrorResponse(
                            dto,
                            ChessGameResponseMessage.InvalidData,
                            System.Net.HttpStatusCode.BadRequest,
                            errorMessages));
            }
            return await Task.FromResult(
                ResponseDTO<TDto, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    dto,
                    ChessGameResponseMessage.SuccessData,
                    System.Net.HttpStatusCode.OK));
        }
    }
}
