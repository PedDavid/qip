using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Output {
    public class OutImage {
        public string type = "image";
        public long Id { get; set; }
        public long BoardId { get; set; }
        public string Src { get; set; }
        public OutPoint Origin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
