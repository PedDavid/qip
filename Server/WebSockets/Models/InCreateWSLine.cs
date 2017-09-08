using IODomain.Input;
using System.ComponentModel.DataAnnotations;

namespace WebSockets.Models {
    public class InCreateWSLine : InCreateLine {
        [Required]
        public long TempId { get; set; }
    }
}
