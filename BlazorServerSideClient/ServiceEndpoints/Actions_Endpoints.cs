namespace ChessGameBlazorClient.ServiceEndpoints
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
            Resign,
            History,
            HistoryPagination,
            GameHistory
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
            ChessGame,
            HistoryWidget
        }

        public enum ChatEndpoints
        {
            Chat
        }
    }
}

