using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutLinePoint : OutPoint {
        [Required]
        public int Idx { get; set; }

        [Required]
        public OutPointStyle Style { get; set; }
    }
}
