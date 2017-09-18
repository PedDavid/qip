using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QIP.IODomain.Input {
    public class InCreateBoard {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        //[DefaultValue()] //TODO - a não satisfação implica um nullable no model
        public byte? MaxDistPoints { get; set; }

        [Range(0, 2)]
        [DefaultValue(0)]
        public InBoardPermission BasePermission { get; set; }
    }
}
