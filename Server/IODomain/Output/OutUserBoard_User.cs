using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutUserBoard_User {
        [Required]
        public OutUser User { get; set; }

        [Required]
        public OutBoardPermission Permission { get; set; }
    }
}
