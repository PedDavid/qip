using API.Models;
using API.Models.Extensions;
using API.Models.Input;
using API.Models.IRepositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace API.WebSockets {
    public class WSBoardOperationsFactory {
        private readonly ILineRepository _lineRepository;
        private readonly IImageRepository _imageRepository;

        public WSBoardOperationsFactory(IImageRepository imageRepository, ILineRepository lineRepository) {
            this._imageRepository = imageRepository;
            this._lineRepository = lineRepository;
        }

        public Dictionary<Action, Func<dynamic, Task<int>>> Generate() {
            var operations = new Dictionary<Action, Func<dynamic, Task<int>>> {
                { Action.CREATE_IMAGE, CreateImage }
            };
            return operations;
        }

        public async Task<int> CreateImage(dynamic inputImage) {//todo verificar se pode ser private
            await Task.CompletedTask;
            int status = 200;

            //TODO Refazer correctamente
            //InImage inImage = inputImage.Value as InImage;

            //if(!inputImage is InImage || !inputImage.Id.HasValue) {
            //    return 400;
            //}

            //Image image = new Image(inputImage.boardId, inputImage.Id.Value).In(inImage);
            //long id = _imageRepository.Add(image);

            return status;
        }
    }
}
