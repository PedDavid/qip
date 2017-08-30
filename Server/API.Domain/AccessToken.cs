using System;

namespace API.Domain {
    public class AccessToken {
        public string Access_token { get; set; }
        public long Expires_in { get; set; }
        public string Scope { get; set; }
        public string Token_type { get; set; }
    }
}
