using API.Domain;
using IODomain.Input;
using IODomain.Output;

namespace IODomain.Extensions {
    public static class BoardExtensions {
        public static OutBoard Out(this Board board) {
            return new OutBoard() {
                Id = board.Id,
                Name = board.Name,
                MaxDistPoints = board.MaxDistPoints,
                BasePermission = BoardPermissionConverter.ConvertToOut(board.BasePermission)
            };
        }

        public static Board In(this Board board, InCreateBoard inBoard) {
            board.Name = inBoard.Name;
            board.MaxDistPoints = inBoard.MaxDistPoints.Value;
            board.BasePermission = BoardPermissionConverter.ConvertFromIn(inBoard.BasePermission);
            return board;
        }

        public static Board In(this Board board, InUpdateBoard inBoard) {
            board.Name = inBoard.Name;
            board.MaxDistPoints = inBoard.MaxDistPoints.Value;
            board.BasePermission = BoardPermissionConverter.ConvertFromIn(inBoard.BasePermission);
            return board;
        }
    }
}
