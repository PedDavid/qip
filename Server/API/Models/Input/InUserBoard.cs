using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Input {
    public class InUserBoard {
        public long? UserId { get; set; }
        public long? BoardId { get; set; }
        public InBoardPermission Permission { get; set; }
    }
}
