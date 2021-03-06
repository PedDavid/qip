﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InUpdateLine {
        [Required]
        [Range(0, long.MaxValue)]
        public long? Id { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long? BoardId { get; set; }

        [Required]
        public IEnumerable<InLinePoint> Points { get; set; }

        [Required]
        public bool? Closed { get; set; }

        [Required]
        public InCreateLineStyle Style { get; set; }
    }
}
