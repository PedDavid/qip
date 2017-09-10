using System.Net;

namespace API.MediaTypes {
    public class ProblemPlusJson {
        public string Type { get; set; } = "about:blank";
        public string Title { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Details { get; set; }
        public string Instance { get; set; }
    }
}
