using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Input {
    public class InUserBoard_Board {
        public InBoard Board { get; set; }
        public InBoardPermission Permission { get; set; }
    }
}
