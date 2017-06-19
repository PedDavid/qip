using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Output {
    public class OutUser {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Favorites { get; set; }
        public string PenColors { get; set; }
    }
}
