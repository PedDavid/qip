﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Output {
    public class OutUserBoard {
        public string UserId { get; set; }
        public long BoardId { get; set; }
        public OutBoardPermission Permission { get; set; }
    }
}
