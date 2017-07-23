using System;
using System.Collections.Generic;
using System.Text;

namespace API.Services.Exceptions {
    class InvalidInputException : Exception {
        public InvalidInputException() : base() {
        }

        public InvalidInputException(string message) : base(message) {
        }
    }
}
