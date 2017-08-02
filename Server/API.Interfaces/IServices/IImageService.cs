using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Interfaces.IServices {
    public interface IImageService : IFigureService<InImage, OutImage> {
    }
}
