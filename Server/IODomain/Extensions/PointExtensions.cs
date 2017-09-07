using API.Domain;
using IODomain.Input;
using IODomain.Output;

namespace IODomain.Extensions {
    public static class PointExtensions {
        public static OutPoint Out(this Point figure) {
            return figure.Out(new OutPoint());
        }
        internal static OutPoint Out(this Point point, OutPoint outPoint) {
            outPoint.X = point.X;
            outPoint.Y = point.Y;
            return outPoint;
        }

        public static Point In(this Point point, InPoint inPoint) {
            point.X = inPoint.X.Value;
            point.Y = inPoint.Y.Value;
            return point;
        }
    }
}
