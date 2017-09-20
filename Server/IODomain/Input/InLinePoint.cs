using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InLinePoint : InPoint {
        [Required]
        public int? Idx { get; set; }

        [Required]
        public InPointStyle Style { get; set; }
    }
}
