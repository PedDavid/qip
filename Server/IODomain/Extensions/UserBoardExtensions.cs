using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class UserBoardExtensions {
        public static OutUserBoard Out(this UserBoard userBoard) {
            return new OutUserBoard() {
                UserId = userBoard.UserId,
                BoardId = userBoard.BoardId.Value,
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }

        public static UserBoard In(this UserBoard userBoard, InUserBoard inUserBoard) {
            userBoard.UserId = inUserBoard.UserId;
            userBoard.BoardId = inUserBoard.BoardId;
            userBoard.Permission = BoardPermissionConverter.ConvertFromIn(inUserBoard.Permission);

            return userBoard;
        }
    }
}
