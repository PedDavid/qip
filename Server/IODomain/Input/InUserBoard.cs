using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Input {
    public class InUserBoard {
        public string UserId { get; set; }
        public long? BoardId { get; set; }
        public InBoardPermission Permission { get; set; }
    }
}
