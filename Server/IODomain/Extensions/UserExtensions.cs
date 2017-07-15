using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class UserExtensions {
        public static OutUser Out(this User user) {
            return new OutUser() {
                Id = user.Id.Value,
                Name = user.Name,
                UserName = user.UserName,
                Favorites = user.Favorites,
                PenColors = user.PenColors
            };
        }

        public static OutPartialUser OutPartial(this User user) {
            return new OutPartialUser() {
                Id = user.Id.Value,
                Name = user.Name,
                UserName = user.UserName
            };
        }

        public static User In(this User user, InUser inUser) {
            user.Name = inUser.Name;
            user.PwdHash = inUser.PwdHash;
            user.PwdSalt = inUser.PwdSalt;
            user.UserName = inUser.UserName;
            user.Favorites = inUser.Favorites;
            user.PenColors = inUser.PenColors;

            return user;
        }
    }
}
