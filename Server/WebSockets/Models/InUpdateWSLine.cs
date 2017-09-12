using IODomain.Input;

namespace WebSockets.Models {
    public class InUpdateWSLine : InUpdateLine {
        public Offset OffsetPoint { get; set; }

        public string IsScaling { get; set; }
    }
}
