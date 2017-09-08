using IODomain.Input;
using System.ComponentModel.DataAnnotations;

namespace WebSockets.Models {
    public class InUpdateWSLine : InUpdateLine {
        [Required]
        public int OffsetPoint { get; set; }
    }
}
