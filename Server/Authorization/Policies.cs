namespace Authorization {
    public static class Policies {
        public static readonly string UserIsOwnPolicy = "UserIsOwn";
        public static readonly string BoardIsOwnPolicy = "BoardOwner";
        public static readonly string ReadBoardPolicy = "ReadBoard";
        public static readonly string WriteBoadPolicy = "WriteBoad";
    }
}
