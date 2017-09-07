using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InCreateImage {
        [Required]
        public long? BoardId { get; set; }

        [Required]
        public string Src { get; set; }

        [Required]
        public InPoint Origin { get; set; }

        [Required]
        public int? Width { get; set; }

        [Required]
        public int? Height { get; set; }
    }
}
