using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class Line {
        public Line(long boardId, long id) {
            BoardId = boardId;
            Id = id;
            Points = new List<LinePoint>();
        }

        public long Id { get; }
        public long BoardId { get; }
        public IEnumerable<LinePoint> Points { get; set; }
        public bool? Closed { get; set; }
        public LineStyle Style { get; set; }
    }
}
