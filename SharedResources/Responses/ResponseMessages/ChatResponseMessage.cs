using SharedResources.Contracts.RequestsAndResponses;
namespace SharedResources.Responses.ResponseMessages
{

    public class ChatResponseMessage : IMessage
    {
        public string MessageSent => "Message sent successfully.";
        public string MessageFailed => "Failed to send message.";
        public string MessageReceived => "Message received successfully.";
        public string MessageNotFound => "Message not found.";
        public string MessageDeleted => "Message deleted successfully.";
        public string MessageDeleteFailed => "Failed to delete message.";
        public string UserNotInChat => "User is not part of this chat.";
        public string ChatCreated => "Chat created successfully.";
        public string ChatCreationFailed => "Failed to create chat.";
        public string ChatNotFound => "Chat not found.";
        public string InternalServerError => "Internal server error. Please try again later.";
    }
}
