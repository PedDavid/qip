namespace Authorization.Resources {
    public class UserRequest {
        public string UserId { get; }

        public UserRequest(string userId) {
            UserId = userId;
        }

    }
}
