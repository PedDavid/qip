using QIP.Domain;
using QIP.IODomain.Output;

namespace QIP.IODomain.Extensions {
    public static class UserExtensions {
        public static OutUser Out(this User user) {
            return new OutUser() {
                Id = user.User_id,
                Name = user.Name,
                UserName = user.UserName,
                Nickname = user.Nickname,
                Picture = user.Picture
            };
        }
    }
}
