using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class Line {
        public Line(long boardId, long id) {
            BoardId = boardId;
            Id = id;
            Points = new List<LinePoint>();
        }

        public long Id { get; }
        public long BoardId { get; }
        public IEnumerable<LinePoint> Points { get; internal set; }
        public bool? Closed { get; internal set; }
        public LineStyle Style { get; internal set; }
    }
}
