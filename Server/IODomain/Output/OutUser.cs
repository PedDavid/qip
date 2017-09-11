using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutUser {
        [Required]
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Picture { get; set; }

        public string Nickname { get; set; }

        public string Name { get; set; }
    }
}
