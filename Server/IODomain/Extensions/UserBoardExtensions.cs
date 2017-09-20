using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class UserBoardExtensions {
        public static OutUserBoard Out(this UserBoard userBoard) {
            return new OutUserBoard() {
                UserId = userBoard.UserId,
                BoardId = userBoard.BoardId,
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }

        public static UserBoard In(this UserBoard userBoard, InCreateUserBoard inUserBoard) {
            userBoard.UserId = inUserBoard.UserId;
            userBoard.BoardId = inUserBoard.BoardId.Value;
            userBoard.Permission = BoardPermissionConverter.ConvertFromIn(inUserBoard.Permission);
            return userBoard;
        }

        public static UserBoard In(this UserBoard userBoard, InUpdateUserBoard inUserBoard) {
            userBoard.UserId = inUserBoard.UserId;
            userBoard.BoardId = inUserBoard.BoardId.Value;
            userBoard.Permission = BoardPermissionConverter.ConvertFromIn(inUserBoard.Permission.Value);
            return userBoard;
        }
    }
}
