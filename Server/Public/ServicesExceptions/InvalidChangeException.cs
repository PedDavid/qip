using System;

namespace QIP.Public.ServicesExceptions {
    public class InvalidChangeException : ServiceException {
        public InvalidChangeException() : base() { }

        public InvalidChangeException(string message) : base(message) { }
    }
}
