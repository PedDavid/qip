using System;

namespace API.Interfaces.ServicesExceptions {
    public class InvalidChangeException : ServiceException {
        public InvalidChangeException() : base() { }

        public InvalidChangeException(string message) : base(message) { }
    }
}
