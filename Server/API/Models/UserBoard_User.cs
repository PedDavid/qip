using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class UserBoard_User {
        public User User { get; internal set; }
        public BoardPermission Permission { get; internal set; }
    }
}
