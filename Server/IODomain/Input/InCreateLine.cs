using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InCreateLine {
        [Required]
        public long? BoardId { get; set; }

        [Required]
        public IEnumerable<InLinePoint> Points { get; set; }

        [DefaultValue(false)]
        public bool Closed { get; set; }

        [Required]
        public InCreateLineStyle Style { get; set; }
    }
}
