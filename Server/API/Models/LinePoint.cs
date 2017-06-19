using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class LinePoint : Point {
        public int? Idx { get; internal set; }
        public PointStyle Style { get; internal set; }
    }
}
