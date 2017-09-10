using IODomain.Input;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebSockets.Models {
    class InCreateWSImage : InCreateImage {
        [Required]
        public long TempId { get; set; }

        [DefaultValue(false)]
        public bool PersistLocalBoard { get; set; }
    }
}
