using API.Domain;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IODomain.Extensions {
    public static class ImageExtensions {
        public static OutImage Out(this Image image) {
            OutImage outImage = new OutImage() {
                Id = image.Id,
                BoardId = image.BoardId,
                Src = image.Src,
                Origin = image.Origin,
                Width = image.Width.Value,
                Height = image.Height.Value
            };
            return outImage;
        }

        public static Image In(this Image image, InImage inImage) {
            image.Src = inImage.Src;
            image.Origin = (image.Origin ?? new Point()).In(inImage.Origin);
            image.Width = inImage.Width;
            image.Height = inImage.Height;

            return image;
        }
    }
}
