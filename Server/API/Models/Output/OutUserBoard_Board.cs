using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutUserBoard_Board {
        public OutBoard Board { get; set; }
        public OutBoardPermission Permission { get; set; }
    }
}
