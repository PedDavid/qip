using System;

namespace API.Interfaces.ServicesExceptions {
    public class InvalidFieldsException : Exception {
        public InvalidFieldsException() : base() {
        }

        public InvalidFieldsException(string message) : base(message) {
        }
    }
}
