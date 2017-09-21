using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InUpdateUserBoard {
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long? BoardId { get; set; }

        [Required]
        [Range(1, 2)]
        public InBoardPermission? Permission { get; set; }
    }
}
