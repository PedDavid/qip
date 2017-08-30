using System;

namespace API.Interfaces.ServicesExceptions {
    public class InconsistentRequestException : Exception {
        public InconsistentRequestException() : base() {
        }

        public InconsistentRequestException(string message) : base(message) {
        }
    }
}
