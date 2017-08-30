using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class BoardExtensions {
        public static OutBoard Out(this Board board) {
            return new OutBoard() {
                Id = board.Id.Value,
                Name = board.Name,
                MaxDistPoints = board.MaxDistPoints.Value
            };
        }

        public static OutBoard Out(this Board board, long id) {
            return new OutBoard() {
                Id = board.Id??id,
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
