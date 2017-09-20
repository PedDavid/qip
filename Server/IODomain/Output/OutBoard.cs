using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutBoard {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public byte MaxDistPoints { get; set; }

        [Required]
        public OutBoardPermission BasePermission { get; set; }
    }
}
