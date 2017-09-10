using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InUpdateImage {
        [Required]
        public long? Id { get; set; }

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
