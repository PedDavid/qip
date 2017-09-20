using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InPreferences {
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public string Favorites { get; set; }

        public string PenColors { get; set; }

        [StringLength(256)]
        public string DefaultPen { get; set; }

        [StringLength(256)]
        public string DefaultEraser { get; set; }

        [StringLength(256)]
        public string CurrTool { get; set; }

        [StringLength(256)]
        public string Settings { get; set; }
    }
}
