using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class UserBoard_User {
        public User User { get; set; }
        public BoardPermission Permission { get; set; }
    }
}
