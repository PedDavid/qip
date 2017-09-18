using System.ComponentModel.DataAnnotations;

namespace QIP.WebSockets.Models {
    public class Offset {
        [Required]
        public int? X { get; set; }

        [Required]
        public int? Y { get; set; }
    }
}
