using API.Domain;
using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class PointExtensions {
        public static OutPoint Out(this Point figure) {
            return figure.Out(new OutPoint());
        }
        internal static OutPoint Out(this Point point, OutPoint outPoint) {
            outPoint.X = point.X.Value;
            outPoint.Y = point.Y.Value;

            return outPoint;
        }

        public static Point In(this Point point, InPoint inPoint) {
            point.X = inPoint.X;
            point.Y = inPoint.Y;

            return point;
        }
    }
}
