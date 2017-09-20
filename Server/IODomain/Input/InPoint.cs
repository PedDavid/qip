using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InPoint {
        [Required]
        public int? X { get; set; }

        [Required]
        public int? Y { get; set; }
    }
}
