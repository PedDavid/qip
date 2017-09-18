using System;

namespace QIP.Public.ServicesExceptions {
    public class NotFoundException : ServiceException {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }
    }
}
