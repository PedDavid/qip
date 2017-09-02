using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class BoardPermissionConverter {
        public static OutBoardPermission ConvertToOut(BoardPermission board) {
            OutBoardPermission outBoard;
            switch(board) {
                case BoardPermission.View: outBoard = OutBoardPermission.View; break;
                case BoardPermission.Edit: outBoard = OutBoardPermission.Edit; break;
                case BoardPermission.Owner: outBoard = OutBoardPermission.Owner; break;
                default: outBoard = default(OutBoardPermission); break;
            }

            return outBoard;
        }

        public static BoardPermission ConvertFromIn(InBoardPermission inBoard) {
            BoardPermission board;
            switch(inBoard) {
                case InBoardPermission.View: board = BoardPermission.View; break;
                case InBoardPermission.Edit: board = BoardPermission.Edit; break;
                case InBoardPermission.Owner: board = BoardPermission.Owner; break;
                default: board = default(BoardPermission); break;
            }

            return board;
        }
    }
}
