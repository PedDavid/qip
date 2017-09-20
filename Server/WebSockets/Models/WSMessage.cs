using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace QIP.WebSockets.Models {
    public class WSMessage {
        [Required]
        public OperationType? Type { get; set; }

        [Required]
        public JObject Payload { get; set; }
    }
}
