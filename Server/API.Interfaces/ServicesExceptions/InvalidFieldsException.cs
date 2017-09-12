using System;

namespace API.Interfaces.ServicesExceptions {
    public class InvalidFieldsException : ServiceException {
        public InvalidFieldsException() : base() {
        }

        public InvalidFieldsException(string message) : base(message) {
        }
    }
}
