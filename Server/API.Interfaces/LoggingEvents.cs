namespace API.Interfaces {
    public class LoggingEvents {
        /*
         * Board
         */
        public const int ListBoards = 1000;
        public const int GetBoard = 1001;
        public const int InsertBoard = 1002;
        public const int UpdateBoard = 1003;
        public const int DeleteBoard = 1004;

        public const int GetBoardNotAuthorized = 4004;
        public const int UpdateBoardNotAuthorized = 4004;

        public const int CreateWithoutBody = 4004;
        public const int UpdateWithoutBody = 4004;
        public const int UpdateBoardWrongId= 4004;

        public const int GetBoardNotFound = 4004;
        public const int UpdateBoardNotFound = 4004;
        public const int DeleteBoardNotFound = 4004;

        /*
         * 
         */
    }
}
