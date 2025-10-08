using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Responses.ResponseMessages
{
    public class IdentityResponseMesage : IMessage
    {
        public string MessageOutput { get; }

        private IdentityResponseMesage(string message)
        {
            MessageOutput = message;
        }
        public static readonly IdentityResponseMesage Initialize =
            new IdentityResponseMesage("Initialized IdentityResponseMessage");

        public static readonly IdentityResponseMesage UserCreated =
            new IdentityResponseMesage("User created successfully.");

        public static readonly IdentityResponseMesage UserCreationFailed =
            new IdentityResponseMesage("User creation failed.");

        public static readonly IdentityResponseMesage UserAlreadyExists =
            new IdentityResponseMesage("User already exists.");

        public static readonly IdentityResponseMesage InvalidCredentials =
            new IdentityResponseMesage("Invalid username or password.");

        public static readonly IdentityResponseMesage UserSignedIn =
            new IdentityResponseMesage("User signed in successfully.");

        public static readonly IdentityResponseMesage UserSignInFailed =
            new IdentityResponseMesage("User sign-in failed.");

        public static readonly IdentityResponseMesage UserSignedOut =
            new IdentityResponseMesage("User signed out successfully.");

        public static readonly IdentityResponseMesage UserNotFound =
            new IdentityResponseMesage("User not found.");

        public static readonly IdentityResponseMesage InternalServerError =
            new IdentityResponseMesage("Internal server error. Please try again later.");

        public static readonly IdentityResponseMesage InvalidaRefreshToken =
            new IdentityResponseMesage("Invalid refresh token.");

        public static readonly IdentityResponseMesage SuccessRefreshToken =
            new IdentityResponseMesage("refresh token Success.");
    }
}
