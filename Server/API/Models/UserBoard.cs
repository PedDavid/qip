using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class UserBoard {
        public long? UserId { get; internal set; }
        public long? BoardId { get; internal set; }
        public BoardPermission Permission { get; internal set; }
    }
}
