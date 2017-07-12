using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Input {
    public class InUser {
        public long? Id { get; set; }
        public string UserName { get; set; }
        public string PwdHash { get; set; }
        public string PwdSalt { get; set; }
        public string Name { get; set; }
        public string Favorites { get; set; }
        public string PenColors { get; set; }
    }
}
