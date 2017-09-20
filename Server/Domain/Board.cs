namespace QIP.Domain {
    public class Board {
        public long Id { get; set; }

        public string Name { get; set; }

        public byte MaxDistPoints { get; set; }

        public BoardPermission BasePermission { get; set; }
    }
}
