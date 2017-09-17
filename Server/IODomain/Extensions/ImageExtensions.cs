using QIP.Domain;
using QIP.IODomain.Input;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class ImageExtensions {
        public static OutImage Out(this Image image) {
            OutImage outImage = new OutImage() {
                Id = image.Id,
                BoardId = image.BoardId,
                Src = image.Src,
                Origin = image.Origin.Out(),
                Width = image.Width,
                Height = image.Height
            };
            return outImage;
        }

        public static Image In(this Image image, InCreateImage inImage) {
            image.BoardId = inImage.BoardId.Value;
            image.Src = inImage.Src;
            image.Origin = (image.Origin ?? new Point()).In(inImage.Origin);
            image.Width = inImage.Width.Value;
            image.Height = inImage.Height.Value;
            return image;
        }

        public static Image In(this Image image, InUpdateImage inImage) {
            image.Src = inImage.Src;
            image.Origin = (image.Origin ?? new Point()).In(inImage.Origin);
            image.Width = inImage.Width.Value;
            image.Height = inImage.Height.Value;
            return image;
        }
    }
}
