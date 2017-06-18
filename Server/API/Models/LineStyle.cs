using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class LineStyle {
        public LineStyle() { }
        public LineStyle(long id) {
            Id = id;
        }

        public long? Id { get; }
        public string Color { get; internal set; }
    }
}
