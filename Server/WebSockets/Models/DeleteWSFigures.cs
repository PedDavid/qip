using System.ComponentModel.DataAnnotations;

namespace QIP.WebSockets.Models {
    public class DeleteWSFigures {
        [Required]
        public long BoardId { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long MaxFigureId { get; set; }
    }
}
