namespace QIP.Domain {
    public class Image {
        public long Id { get; set; }

        public long BoardId { get; set; }

        public string Src { get; set; }

        public Point Origin { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
