using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Input {
    public class InImage {
        public long? Id { get; set; }
        public long? BoardId { get; set; }
        public string Src { get; set; }
        public InPoint Origin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
