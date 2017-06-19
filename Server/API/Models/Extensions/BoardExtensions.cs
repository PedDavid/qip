using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class BoardExtensions {
        public static OutBoard Out(this Board board) {
            return new OutBoard() {
                Id = board.Id.Value,
                Name = board.Name,
                MaxDistPoints = board.MaxDistPoints.Value
            };
        }

        public static Board In(this Board board, InBoard inBoard) {
            board.Name = inBoard.Name;
            board.MaxDistPoints = inBoard.MaxDistPoints;

            return board;
        }
    }
}
