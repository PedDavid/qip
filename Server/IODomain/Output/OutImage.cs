using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutImage {
        [Required]
        public string type = "image";

        [Required]
        public long Id { get; set; }

        [Required]
        public long BoardId { get; set; }

        [Required]
        public string Src { get; set; }

        [Required]
        public OutPoint Origin { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }
    }
}
