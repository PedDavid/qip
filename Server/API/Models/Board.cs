using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class Board {
        public Board() { }

        public Board(long id) {
            Id = id;
        }

        public long? Id { get; }
        public string Name { get; internal set; }
        public byte? MaxDistPoints { get; internal set; }
    }
}
