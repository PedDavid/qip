using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class PointStyle {
        public PointStyle() { }
        public PointStyle(long id) {
            Id = id;
        }

        public PointStyle(long? id) {
            if(id.HasValue)
                Id = id.Value;
        }

        public long? Id { get; }
        public int? Width { get; set; }
    }
}
