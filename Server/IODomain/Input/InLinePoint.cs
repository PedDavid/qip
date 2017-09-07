using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InLinePoint : InPoint {
        [Required]
        public int? Idx { get; set; }

        [Required]
        public InPointStyle Style { get; set; }
    }
}
