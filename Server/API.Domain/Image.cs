using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class Image {
        public Image(long boardId, long id) {
            BoardId = boardId;
            Id = id;
        }

        public long Id { get; }
        public long BoardId { get; }
        public string Src { get; set; }
        public Point Origin { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
