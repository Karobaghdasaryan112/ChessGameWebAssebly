using FluentValidation;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.ConnectionResponses;

namespace ChessGame.Core.Services.Services.Validations
{
    public class ConnectionValidationService
    {
        public ConnectionValidationService
        (
            //Requests
            IValidator<AddUserConnectionRequestDTOValidation> addUserConnectionValidator,
            IValidator<GetUserConnectionRequestDTOValidation> getUserConnectionValidator,
            IValidator<RemoveUserConnectionRequestDTOValidation> removeUserConnectionValidator,

            //Responses
            IValidator<GetUserConnectionResponseDTOValidation> getUserConnectionResponseValidator,
            IValidator<AddUserConnectionResponseDTOValidation> addUserConnectionResponseValidator,
            IValidator<RemoveUserConnectionResponseDTOValidation> removeUserConnectionResponseValidator
        )
        {

        }

    }
}
