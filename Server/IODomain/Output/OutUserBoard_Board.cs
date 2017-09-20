using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutUserBoard_Board {
        [Required]
        public OutBoard Board { get; set; }

        [Required]
        public OutBoardPermission Permission { get; set; }
    }
}
