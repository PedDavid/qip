namespace API.Domain {
    public class UserBoard {
        public string UserId { get; set; }
        public long BoardId { get; set; }
        public BoardPermission Permission { get; set; }
    }
}
