using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class UserBoard_UserExtensions {
        public static OutUserBoard_User Out(this UserBoard_User userBoard) {
            return new OutUserBoard_User() {
                User = userBoard.User.Out(),
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }
    }
}
