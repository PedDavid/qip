using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InUpdateLineStyle {
        [Required]
        public long? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; }
    }
}
