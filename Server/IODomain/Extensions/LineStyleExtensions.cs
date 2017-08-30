using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class LineStyleExtensions {
        public static OutLineStyle Out(this LineStyle lineStyle) {
            return new OutLineStyle() {
                Id = lineStyle.Id??-1,//TODO CHANGE
                Color = lineStyle.Color
            };
        }

        public static OutLineStyle Out(this LineStyle lineStyle, long id) {
            return new OutLineStyle() {
                Id = lineStyle.Id ?? id,
                Color = lineStyle.Color
            };
        }

        public static LineStyle In(this LineStyle lineStyle, InLineStyle inLineStyle) {
            lineStyle.Color = inLineStyle.Color;

            return lineStyle;
        }
    }
}
