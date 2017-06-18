using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutUserBoard_User {
        public OutPartialUser User { get; set; }
        public OutBoardPermission Permission { get; set; }
    }
}
