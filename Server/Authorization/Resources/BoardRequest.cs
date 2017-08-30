namespace Authorization.Resources {
    public class BoardRequest {
        public long BoardId {get;}

        public BoardRequest(long boardId) {
            BoardId = boardId;
        }
    }
}
