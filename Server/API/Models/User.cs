using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    public class User {
        public User() { }

        public User(long id) {
            Id = id;
        }

        public long? Id { get; }
        public string UserName { get; internal set; }
        public string PwdHash { get; internal set; }
        public string PwdSalt { get; internal set; }
        public string Name { get; internal set; }
        public string Favorites { get; internal set; }
        public string PenColors { get; internal set; }
    }
}
