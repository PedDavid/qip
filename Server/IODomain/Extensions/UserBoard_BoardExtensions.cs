using QIP.Domain;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class UserBoard_BoardExtensions {
        public static OutUserBoard_Board Out(this UserBoard_Board userBoard) {
            return new OutUserBoard_Board() {
                Board = userBoard.Board.Out(),
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }
    }
}
