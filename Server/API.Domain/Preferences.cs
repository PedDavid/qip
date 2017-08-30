namespace API.Domain {
    public class Preferences {
        public Preferences(string userId) {
            UserId = userId;
        }

        public string UserId { get; }
        public string Favorites { get; set; }
        public string PenColors { get; set; }
    }
}
