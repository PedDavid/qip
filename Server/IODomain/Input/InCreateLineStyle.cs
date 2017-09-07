using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InCreateLineStyle {
        [Required]
        [StringLength(20)]
        public string Color { get; set; }
    }
}
