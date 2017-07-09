using API.Interfaces.IRepositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public class ImageOperations {
        private readonly IImageRepository _imageRepository;

        public ImageOperations(IImageRepository imageRepository) {
            _imageRepository = imageRepository;
        }

        //TODO implement

        public async Task<dynamic> CreateImage(JToken inputImage) {
            await Task.CompletedTask;

            //InImage inImage = inputImage.ToObject<InImage>();

            return Operations.EMPTY_RESPONSE;
        }
    }
}
