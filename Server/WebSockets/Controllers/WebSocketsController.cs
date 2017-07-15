using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSockets.Extensions;
using WebSockets.StringWebSockets;
using API.Interfaces.IRepositories;
using WebSockets.Operations;
using API.Services;
using Newtonsoft.Json.Linq;

namespace WebSockets.Controllers {
    [Route("api/[controller]")]
    public class WebSocketsController : Controller {
        private StringWebSocketsSessionManager _sessionManager;
        private readonly LineOperations _lineOperations;
        private readonly ImageOperations _imageOperations;

        public WebSocketsController(
            StringWebSocketsSessionManager sessionManager, 
            IFigureIdRepository figureIdRepository, 
            IImageRepository imageRepository, 
            ILineRepository lineRepository, 
            FigureIdGenerator idGenerator
        ) {
            _sessionManager = sessionManager;
            _imageOperations = new ImageOperations(imageRepository, idGenerator);
            _lineOperations = new LineOperations(lineRepository, idGenerator);
        }

        [HttpGet]
        public async Task Index(long? id) {
            if(HttpContext.WebSockets.IsWebSocketRequest && id.HasValue) {
                StringWebSocket webSocket = await HttpContext.WebSockets.AcceptStringWebSocketAsync();

                long vid = id.Value;
                var swsopers = new StringWebSocketsOperations(webSocket) {
                    Session = _sessionManager.Register(vid, webSocket),
                    ClientId = vid
                };

                LoadActionOperations(swsopers);

                await swsopers.AcceptRequests();
            }
            else {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private void LoadActionOperations(StringWebSocketsOperations operations) {
            operations.AddOperation(Models.Action.CREATE_IMAGE, _imageOperations.CreateImage);
            operations.AddOperation(Models.Action.DELETE_IMAGE, _imageOperations.DeleteImage);
            operations.AddOperation(Models.Action.ALTER_IMAGE, _imageOperations.UpdateImage);
            operations.AddOperation(Models.Action.CREATE_LINE, _lineOperations.CreateLine);
            operations.AddOperation(Models.Action.DELETE_LINE, _lineOperations.DeleteLine);
            operations.AddOperation(Models.Action.ALTER_LINE, _lineOperations.UpdateLine);
        }
    }
}
