using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Input {
    public class InLine {
        public long? Id { get; set; }
        public long? BoardId { get; set; }
        public IEnumerable<InLinePoint> Points { get; set; }
        public bool? Closed { get; set; }
        public InLineStyle Style { get; set; }
    }
}
