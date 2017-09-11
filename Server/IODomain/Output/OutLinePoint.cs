using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutLinePoint : OutPoint {
        [Required]
        public int Idx { get; set; }

        [Required]
        public OutPointStyle Style { get; set; }
    }
}
