using System;
using System.Collections.Generic;
using System.Text;

namespace API.Services.Exceptions {
    class InvalidFieldsException : Exception {
        public InvalidFieldsException() : base() {
        }

        public InvalidFieldsException(string message) : base(message) {
        }
    }
}
