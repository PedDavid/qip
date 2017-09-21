using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InUpdateImage {
        [Required]
        [Range(0, long.MaxValue)]
        public long? Id { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
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
