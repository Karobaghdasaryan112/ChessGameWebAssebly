namespace ServerSideClientUI.ServiceEndpoints
{
    public static class Actions
    {
        public enum IdentityAction
        {
            Register,
            Login,
            RefreshToken,
            GetUsersByIds
        }

        public enum UserAction
        {
            GetAll,
            BanUser,
            DeleteUser
        }

        public enum ChessGameAction
        {
            Start,
            Move,
            Resign
        }

        public enum ChatAction
        {
            SendMessage,
            GetHistory
        }
        public enum Unexpected
        {
            None
        }
    }

    public static class Endpoints
    {
        public enum IdentityEndpoints
        {
            Identity,
            Users
        }

        public enum ChessGameEndpoints
        {
            ChessGame
        }

        public enum ChatEndpoints
        {
            Chat
        }
    }
}

