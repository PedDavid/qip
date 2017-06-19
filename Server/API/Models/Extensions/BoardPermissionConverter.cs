using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class BoardPermissionConverter {
        public static OutBoardPermission ConvertToOut(BoardPermission board) {
            return (OutBoardPermission)board;
        }

        public static BoardPermission ConvertFromIn(InBoardPermission inBoard) {
            return (BoardPermission)inBoard;
        }
    }
}
