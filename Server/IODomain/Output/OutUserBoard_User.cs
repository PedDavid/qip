using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutUserBoard_User {
        [Required]
        public OutUser User { get; set; }

        [Required]
        public OutBoardPermission Permission { get; set; }
    }
}
