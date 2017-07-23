using System;
using System.Collections.Generic;
using System.Text;

namespace API.Services.Exceptions {
    class NotFoundException :Exception{
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }
    }
}
