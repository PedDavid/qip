using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class Image {
        public Image(long boardId, long id) {
            BoardId = boardId;
            Id = id;
        }

        public long Id { get; }
        public long BoardId { get; }
        public string Src { get; internal set; }
        public Point Origin { get; internal set; }
        public int? Width { get; internal set; }
        public int? Height { get; internal set; }
    }
}
