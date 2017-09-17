using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutLine {
        [Required]
        public string type = "figure";

        [Required]
        public long Id { get; set; }

        [Required]
        public long BoardId { get; set; }

        [Required]
        public IEnumerable<OutLinePoint> Points { get; set; }

        [Required]
        public bool Closed { get; set; }

        [Required]
        public OutLineStyle Style { get; set; }
    }
}
