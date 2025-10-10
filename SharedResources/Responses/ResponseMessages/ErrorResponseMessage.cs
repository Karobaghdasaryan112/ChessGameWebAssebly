using SharedResources.Contracts.RequestsAndResponses;
using System.Text.Json.Serialization;

namespace SharedResources.Responses.ResponseMessages
{
    public class ErrorResponseMessage : IMessage
    {
        public string MessageOutput { get; set; }

        [JsonConstructor]
        public ErrorResponseMessage()
        {
        }

        public ErrorResponseMessage(string message)
        {
            MessageOutput = message;
        }

        public override string ToString() => MessageOutput;

        public static readonly ErrorResponseMessage UserNotFound =
            new ErrorResponseMessage("User not found.");

        public static readonly ErrorResponseMessage InvalidCredentials =
            new ErrorResponseMessage("Invalid username or password.");

        public static readonly ErrorResponseMessage UserAlreadyExists =
            new ErrorResponseMessage("A user with the given username already exists.");

        public static readonly ErrorResponseMessage PasswordTooWeak =
            new ErrorResponseMessage("The provided password does not meet the complexity requirements.");

        public static readonly ErrorResponseMessage EmailAlreadyInUse =
            new ErrorResponseMessage("This email address is already associated with another account.");

        public static readonly ErrorResponseMessage InternalServerError =
            new ErrorResponseMessage("An unexpected error occurred on the server.");

        public static readonly ErrorResponseMessage RequestCanceled =
            new ErrorResponseMessage("The request was canceled.");

        public static readonly ErrorResponseMessage ValidationFailed =
            new ErrorResponseMessage("One or more validation errors occurred.");

        public static readonly ErrorResponseMessage UnauthorizedAccess =
            new ErrorResponseMessage("Unauthorized access.");

        public static readonly ErrorResponseMessage Forbidden =
            new ErrorResponseMessage("You do not have permission to perform this action.");

        public static readonly ErrorResponseMessage TokenExpired =
            new ErrorResponseMessage("The token has expired or is invalid.");
    }
}
