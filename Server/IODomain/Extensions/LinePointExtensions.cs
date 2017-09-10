using API.Domain;
using IODomain.Input;
using IODomain.Output;

namespace IODomain.Extensions {
    public static class LinePointExtensions {
        public static OutLinePoint Out(this LinePoint point) {
            var outpoint = new OutLinePoint() {
                Style = point.Style.Out(),
                Idx = point.Idx
            };
            return (OutLinePoint)((Point)point).Out(outpoint);
        }

        public static LinePoint In(this LinePoint point, InLinePoint inPoint) {
            ((Point)point).In(inPoint);
            point.Style = (point.Style ?? new PointStyle()).In(inPoint.Style);
            point.Idx = inPoint.Idx.Value;
            return point;
        }
    }
}
