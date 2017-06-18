using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutUserBoard {
        public long UserId { get; set; }
        public long BoardId { get; set; }
        public OutBoardPermission Permission { get; set; }
    }
}
