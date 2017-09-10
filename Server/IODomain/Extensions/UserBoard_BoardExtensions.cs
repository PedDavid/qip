using API.Domain;
using IODomain.Output;

namespace IODomain.Extensions {
    public static class UserBoard_BoardExtensions {
        public static OutUserBoard_Board Out(this UserBoard_Board userBoard) {
            return new OutUserBoard_Board() {
                Board = userBoard.Board.Out(),
                Permission = BoardPermissionConverter.ConvertToOut(userBoard.Permission)
            };
        }
    }
}
