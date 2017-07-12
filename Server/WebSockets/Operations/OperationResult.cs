using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public class OperationResult {
        public static OperationResult EMPTY = new OperationResult();

        public OperationResult(JToken response, JObject broadcastMessage) {
            Response = response;
            BroadcastMessage = broadcastMessage;
        }

        public OperationResult(JObject broadcastMessage) {
            BroadcastMessage = broadcastMessage;
        }

        public OperationResult(JToken response) {
            Response = response;
        }

        public OperationResult() { }

        public JObject BroadcastMessage { get; }
        public JToken Response { get; }
    }
}
