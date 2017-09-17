namespace QIP.Repositories.Options {
    public class Auth0Options {
        public string Domain { get; set; }

        public ManagementOptions Management { get; set; }

        public class ManagementOptions {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}