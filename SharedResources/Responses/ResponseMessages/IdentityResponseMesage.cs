using SharedResources.Contracts.RequestsAndResponses;

namespace SharedResources.Responses.ResponseMessages
{
    public class IdentityResponseMesage : IMessage
    {
        public string messageOutput { get; set; }
        public IdentityResponseMesage(string message)
        {
            messageOutput = message;
        }

        public static IdentityResponseMesage UserCreated =>  new IdentityResponseMesage("User created successfully.");
        public static IdentityResponseMesage UserCreationFailed => new IdentityResponseMesage("User creation failed.");
        public static IdentityResponseMesage UserAlreadyExists => new IdentityResponseMesage("User already exists.");
        public static IdentityResponseMesage InvalidCredentials => new IdentityResponseMesage("Invalid username or password.");
        public static IdentityResponseMesage UserSignedIn => new IdentityResponseMesage("User signed in successfully.");
        public static IdentityResponseMesage UserSignInFailed => new IdentityResponseMesage("User sign-in failed.");
        public static IdentityResponseMesage UserSignedOut => new IdentityResponseMesage("User signed out successfully.");
        public static IdentityResponseMesage UserNotFound => new IdentityResponseMesage("User not found.");
        public static IdentityResponseMesage InternalServerError => new IdentityResponseMesage("Internal server error. Please try again later.");
        public static IdentityResponseMesage InvalidaRefreshToken => new IdentityResponseMesage("Invalid refresh token.");

    }
}
