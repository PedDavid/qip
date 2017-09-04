using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class Board {
        public Board() { }

        public Board(long id) {
            Id = id;
        }

        public long? Id { get; }
        public string Name { get; set; }
        public byte? MaxDistPoints { get; set; }
        public BoardPermission BasePermission { get; set; }
    }
}
