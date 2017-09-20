using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class LineStyleExtensions {
        public static OutLineStyle Out(this LineStyle lineStyle) {
            return new OutLineStyle() {
                Id = lineStyle.Id,
                Color = lineStyle.Color
            };
        }

        public static LineStyle In(this LineStyle lineStyle, InCreateLineStyle inLineStyle) {
            lineStyle.Color = inLineStyle.Color;

            return lineStyle;
        }

        public static LineStyle In(this LineStyle lineStyle, InUpdateLineStyle inLineStyle) {
            lineStyle.Color = inLineStyle.Color;

            return lineStyle;
        }
    }
}
