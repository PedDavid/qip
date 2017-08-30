using System;

namespace API.Interfaces.ServicesExceptions {
    public class InvalidChangeException : Exception {
        public InvalidChangeException() : base() { }

        public InvalidChangeException(string message) : base(message) { }
    }
}
