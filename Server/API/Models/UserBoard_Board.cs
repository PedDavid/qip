using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class UserBoard_Board {
        public Board Board { get; internal set; }
        public BoardPermission Permission { get; internal set; }
    }
}
