using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InUpdateLineStyle {
        [Required]
        [Range(0, long.MaxValue)]
        public long? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; }
    }
}
