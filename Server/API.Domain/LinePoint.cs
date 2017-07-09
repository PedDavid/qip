using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class LinePoint : Point {
        public int? Idx { get; set; }
        public PointStyle Style { get; set; }
    }
}
