using System.ComponentModel.DataAnnotations;

namespace IODomain.Output {
    public class OutUserBoard {
        [Required]
        public string UserId { get; set; }

        [Required]
        public long BoardId { get; set; }

        [Required]
        public OutBoardPermission Permission { get; set; }
    }
}
