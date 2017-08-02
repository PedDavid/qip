using System;

namespace API.Interfaces.ServicesExceptions {
    public class MissingInputException : Exception {
        public MissingInputException() : base() {
        }

        public MissingInputException(string message) : base(message) {
        }
    }
}
