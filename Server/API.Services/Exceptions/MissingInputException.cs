using System;
using System.Collections.Generic;
using System.Text;

namespace API.Services.Exceptions {
    class MissingInputException : Exception {
        public MissingInputException() : base() {
        }

        public MissingInputException(string message) : base(message) {
        }
    }
}
