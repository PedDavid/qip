﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Input {
    public class InBoard {
        public long? Id { get; set; }

        public string Name { get; set; }
        public byte? MaxDistPoints { get; set; }
    }
}