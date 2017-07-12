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
                User = userBoard.User.OutPartial(),
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }

        public static UserBoard_User In(this UserBoard_User userBoard, InUserBoard_User inUserBoard) {
            userBoard.User.In(inUserBoard.User);
            userBoard.Permission = BoardPermissionConverter.ConvertFromIn(inUserBoard.Permission);

            return userBoard;
        }
    }
}
