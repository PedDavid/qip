using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InCreateUserBoard {
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public long? BoardId { get; set; }

        [Range(1, 2)]
        [DefaultValue(1)]
        public InBoardPermission Permission { get; set; } = InBoardPermission.View;
    }
}
