﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Input {
    public class InLinePoint : InPoint {
        public int? Idx { get; set; }
        public InPointStyle Style { get; set; }
    }
}
