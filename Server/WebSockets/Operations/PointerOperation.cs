using API.Domain;
using API.Interfaces;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebSockets.Extensions;
using WebSockets.Models;
using WebSockets.StringWebSockets;

namespace WebSockets.Operations {
    public class PointerOperation {
        private static readonly JsonSerializerSettings serializerSettings;

        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<PointerOperation> _logger;

        static PointerOperation() {
            serializerSettings = new JsonSerializerSettings() {
                Converters = { new StringEnumConverter() }
            };
        }

        public PointerOperation(IAuthorizationService authorizationService, ILogger<PointerOperation> logger) {
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public async Task PointTo(StringWebSocket stringWebSocket, IStringWebSocketSession session, JObject payload) {
            ClaimsPrincipal user = stringWebSocket.User;
            long boardId = session.Id;

            if(!await _authorizationService.AuthorizeAsync(user, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.PointToNotAuthorized, "PointTo (Board {boardId}) NOT AUTHORIZED {userId}", boardId, user.GetNameIdentifier());
                return;
            }

            InPoint inPoint = payload.ToObject<InPoint>();

            Point point = new Point().In(inPoint);

            _logger.LogInformation(LoggingEvents.PointTo, "Pointing To");

            await session.BroadcastAsync(
               new {
                   type = OperationType.POINT_TO,
                   payload = new { figure = point.Out() }
               },
               serializerSettings
            );
        }
    }
}
