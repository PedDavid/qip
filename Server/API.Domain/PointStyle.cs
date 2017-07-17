﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class PointStyle {
        public PointStyle() { }

        public int? Width { get; set; }

        // todo: use automatic way to convert to json (Try JavaScriptSerializable)
        public string ToJson() {
            return $"{{\"width\": {Width}}}";
        }
    }
}
