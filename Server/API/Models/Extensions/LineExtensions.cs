using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class LineExtensions {
        public static OutLine Out(this Line line) {
            return new OutLine() {
                Id = line.Id,
                BoardId = line.BoardId,
                Points = line.Points.Select(LinePointExtensions.Out),
                Closed = line.Closed.Value,
                Style = line.Style.Out()
            };
        }

        public static Line In(this Line line, InLine inLine) {
            line.Points = inLine.Points.Select(inPoint => new LinePoint().In(inPoint));
            line.Closed = inLine.Closed;
            line.Style = (line.Style ?? new LineStyle()).In(inLine.Style);

            return line;
        }
    }
}
