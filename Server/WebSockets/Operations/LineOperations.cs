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
    public class LineOperations {
        private readonly ILineRepository _lineRepository;
        private readonly FigureIdGenerator _idGen;

        public LineOperations(ILineRepository lineRepository, FigureIdGenerator idGen) {
            _lineRepository = lineRepository;
            _idGen = idGen;
        }

        public OperationResult CreateLine(JObject payload) {//TODO Rever se não pomos os checks aos ids e outros como nos controlers
            if(!(payload.TryGetValue("tempId", StringComparison.OrdinalIgnoreCase, out JToken payload_tempId) && payload_tempId.Type == JTokenType.Integer)) {
                return OperationResult.EMPTY;
            }

            long tempId = payload["tempId"].Value<long>();
            payload.Remove("tempId");

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = _idGen.NewId();
            payload["id"] = id;

            InLine inLine = payload.ToObject<InLine>();
            Line line = new Line(boardId, inLine.Id.Value).In(inLine);
            Task store = Task.Run(() => _lineRepository.AddAsync(line)); //TODO Nota: Fazer com async await para o que for cpu-bound ser feito sincronamente
            //TODO Testar se a Task is completed or faulted, in this cases do result for get the result/exception
            //Fazer alguma coisa em caso de excepções

            return new OperationResult(
                    broadcastMessage: payload,
                    response: JObject.FromObject(
                            new { id = id, tempId = tempId }
                        )
                );
        }

        public OperationResult UpdateLine(JObject payload) {

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = payload["id"].Value<long>();

            InLine inLine = payload.ToObject<InLine>();

            Task store = Task.Run(() => {
                Line line = _lineRepository.FindAsync(id, boardId);
                if(line == null) {
                    return;//TODO REVER
                }

                line.In(inLine);

                _lineRepository.UpdateAsync(line);
            });

            return new OperationResult(broadcastMessage: payload);
        }

        public OperationResult DeleteLine(JObject payload) {

            long boardId = payload["clientId"].Value<long>();
            payload.Remove("clientId");

            long id = payload["id"].Value<long>();

            Task store = Task.Run(() => {
                Line line = _lineRepository.FindAsync(id, boardId);
                if(line == null) {
                    return;//TODO REVER
                }
                _lineRepository.RemoveAsync(id, boardId);
             });

            return new OperationResult(broadcastMessage: payload);
        }
    }
}
