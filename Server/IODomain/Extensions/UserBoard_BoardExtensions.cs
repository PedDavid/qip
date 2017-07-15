using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IODomain.Extensions;
using API.Domain;

namespace IODomain.Extensions {
    public static class UserBoard_BoardExtensions {
        public static OutUserBoard_Board Out(this UserBoard_Board userBoard) {
            return new OutUserBoard_Board() {
                Board = userBoard.Board.Out(),
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }

        public static UserBoard_Board In(this UserBoard_Board userBoard, InUserBoard_Board inUserBoard) {
            userBoard.Board.In(inUserBoard.Board);
            userBoard.Permission = BoardPermissionConverter.ConvertFromIn(inUserBoard.Permission);

            return userBoard;
        }
    }
}
