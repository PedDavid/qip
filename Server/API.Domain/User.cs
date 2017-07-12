using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public class User {
        public User() { }

        public User(long id) {
            Id = id;
        }

        public long? Id { get; }
        public string UserName { get; set; }
        public string PwdHash { get; set; }
        public string PwdSalt { get; set; }
        public string Name { get; set; }
        public string Favorites { get; set; }
        public string PenColors { get; set; }
    }
}
