using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;
using System.Linq;

namespace QIP.IODomain.Extensions {
    public static class LineExtensions {
        public static OutLine Out(this Line line) {
            return new OutLine() {
                Id = line.Id,
                BoardId = line.BoardId,
                Points = line.Points.Select(LinePointExtensions.Out),
                Closed = line.Closed,
                Style = line.Style.Out()
            };
        }

        public static Line In(this Line line, InCreateLine inLine) {
            line.BoardId = inLine.BoardId.Value;
            line.Points = inLine.Points.Select(inPoint => new LinePoint().In(inPoint));
            line.Closed = inLine.Closed;
            line.Style = (line.Style ?? new LineStyle()).In(inLine.Style);
            return line;
        }

        public static Line In(this Line line, InUpdateLine inLine) {
            line.Points = inLine.Points.Select(inPoint => new LinePoint().In(inPoint));
            line.Closed = inLine.Closed.Value;
            line.Style = (line.Style ?? new LineStyle()).In(inLine.Style);
            return line;
        }
    }
}
