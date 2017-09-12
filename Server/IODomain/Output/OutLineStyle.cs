using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutLineStyle {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [Required]
        public string Color { get; set; }
    }
}
