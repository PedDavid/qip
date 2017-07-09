﻿using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutImage {
        public long Id { get; set; }
        public long BoardId { get; set; }
        public string Src { get; set; }
        public Point Origin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
