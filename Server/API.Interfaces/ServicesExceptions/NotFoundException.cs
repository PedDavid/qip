using System;

namespace API.Interfaces.ServicesExceptions {
    public class NotFoundException : Exception {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }
    }
}
