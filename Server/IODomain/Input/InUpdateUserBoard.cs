using System.ComponentModel.DataAnnotations;

namespace IODomain.Input {
    public class InUpdateUserBoard {
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public long? BoardId { get; set; }

        [Required]
        [Range(1, 2)]
        public InBoardPermission Permission { get; set; }
    }
}
