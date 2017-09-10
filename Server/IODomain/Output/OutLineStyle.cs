using Newtonsoft.Json;

namespace IODomain.Output {
    public class OutLineStyle {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }
        public string Color { get; set; }
    }
}
