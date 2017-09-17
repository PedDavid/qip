using System.ComponentModel.DataAnnotations;

namespace WebSockets.Models {
    public class DeleteWSFigures {
        [Required]
        public long BoardId { get; set; }

        [Required]
        public long MaxFigureId { get; set; }
    }
}
