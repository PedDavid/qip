﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutLinePoint : OutPoint {
        public int Idx { get; set; }
        public OutPointStyle Style { get; set; }
    }
}