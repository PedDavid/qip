﻿using API.Domain;
using API.Models.Input;
using API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Extensions {
    public static class PointStyleExtensions {
        public static OutPointStyle Out(this PointStyle pointStyle) {
            return new OutPointStyle() {
                Id = pointStyle.Id.Value,
                Width = pointStyle.Width.Value
            };
        }

        public static PointStyle In(this PointStyle pointStyle, InPointStyle inPointStyle) {
            pointStyle.Width = inPointStyle.Width;

            return pointStyle;
        }
    }
}
