using API.Domain;
using API.Interfaces.IRepositories;
using API.Services;
using IODomain.Extensions;
using IODomain.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public class ImageOperations {
        private readonly IImageRepository _imageRepository;
        private readonly FigureIdGenerator _idGen;

        public ImageOperations(IImageRepository imageRepository, FigureIdGenerator idGen) {
            _imageRepository = imageRepository;
            _idGen = idGen;
        }

        public OperationResult CreateImage(JObject payload) {//TODO Rever se não pomos os checks aos ids e outros como nos controlers
            if(!(payload.TryGetValue("tempId", StringComparison.OrdinalIgnoreCase, out JToken payload_tempId) && payload_tempId.Type == JTokenType.Integer)) {
                return OperationResult.EMPTY;
            }

            long tempId = payload["tempId"].Value<long>();
            payload.Remove("tempId");

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = _idGen.NewId();
            payload["id"] = id;

            InImage inImage = payload.ToObject<InImage>();
            Image image = new Image(boardId, inImage.Id.Value).In(inImage);
            Task store = Task.Run(() => _imageRepository.AddAsync(image)); //TODO Nota: Fazer com async await para o que for cpu-bound ser feito sincronamente
            //TODO Testar se a Task is completed or faulted, in this cases do result for get the result/exception
            //Fazer alguma coisa em caso de excepções

            return new OperationResult(
                    broadcastMessage: payload,
                    response: JObject.FromObject(
                            new {id = id, tempId = tempId }
                        )
                );
        }

        public OperationResult UpdateImage(JObject payload) {

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = payload["id"].Value<long>();

            InImage inImage = payload.ToObject<InImage>();

            Task store = Task.Run(() => {
                Image image = _imageRepository.FindAsync(id, boardId);
                if(image == null) {
                    return;//TODO REVER
                }

                image.In(inImage);

                _imageRepository.UpdateAsync(image);
            });

            return new OperationResult(broadcastMessage: payload);
        }

        public OperationResult DeleteImage(JObject payload) {

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = payload["id"].Value<long>();

            Task store = Task.Run(() => {
                Image image = _imageRepository.FindAsync(id, boardId);
                if(image == null) {
                    return;//TODO REVER
                }
                _imageRepository.RemoveAsync(id, boardId);
             });

            return new OperationResult(broadcastMessage: payload);
        }
    }
}
