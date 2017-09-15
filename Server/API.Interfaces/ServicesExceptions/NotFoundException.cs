﻿using System;

namespace API.Interfaces.ServicesExceptions {
    public class NotFoundException : ServiceException {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }
    }
}
