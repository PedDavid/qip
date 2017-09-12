using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutPoint {
        [Required]
        public int X { get; set; }

        [Required]
        public int Y { get; set; }
    }
}
