using System.Collections.Generic;

namespace QIP.Domain {
    public class Line {
        public long Id { get; set; }

        public long BoardId { get; set; }

        public IEnumerable<LinePoint> Points { get; set; } = new List<LinePoint>();

        public bool Closed { get; set; }

        public LineStyle Style { get; set; }
    }
}
