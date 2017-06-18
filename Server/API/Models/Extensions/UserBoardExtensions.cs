using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class UserBoardExtensions {
        public static OutUserBoard Out(this UserBoard userBoard) {
            return new OutUserBoard() {
                UserId = userBoard.UserId.Value,
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
