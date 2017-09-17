using System;

namespace QIP.Public.ServicesExceptions {
    public class InvalidFieldsException : ServiceException {
        public InvalidFieldsException() : base() {
        }

        public InvalidFieldsException(string message) : base(message) {
        }
    }
}
