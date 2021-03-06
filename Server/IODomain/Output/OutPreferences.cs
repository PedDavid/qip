﻿using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Output {
    public class OutPreferences {
        [Required]
        public string UserId { get; set; }

        public string Favorites { get; set; }

        public string PenColors { get; set; }

        public string DefaultPen { get; set; }

        public string DefaultEraser { get; set; }

        public string CurrTool { get; set; }

        public string Settings { get; set; }
    }
}
