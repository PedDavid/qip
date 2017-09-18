using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class PointStyleExtensions {
        public static OutPointStyle Out(this PointStyle pointStyle) {
            return new OutPointStyle() {
                Width = pointStyle.Width.Value
            };
        }

        public static PointStyle In(this PointStyle pointStyle, InPointStyle inPointStyle) {
            pointStyle.Width = inPointStyle.Width;

            return pointStyle;
        }
    }
}
