﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Input {
    public class InBoard {
        public long? Id { get; set; }

        public string Name { get; set; }
        public byte? MaxDistPoints { get; set; }
        public InBoardPermission BasePermission { get; set; }
    }
}
