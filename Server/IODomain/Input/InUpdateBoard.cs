using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InUpdateBoard {
        [Required]
        [Range(0, long.MaxValue)]
        public long? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public byte? MaxDistPoints { get; set; }

        [Required]
        [Range(0,2)]
        public InBoardPermission? BasePermission { get; set; }
    }
}
