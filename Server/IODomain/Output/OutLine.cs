using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Output {
    public class OutLine {
        public string type = "figure";
        public long Id { get; set; }
        public long BoardId { get; set; }
        public IEnumerable<OutLinePoint> Points { get; set; }
        public bool Closed { get; set; }
        public OutLineStyle Style { get; set; }
    }
}
