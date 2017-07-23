using System;
using System.Collections.Generic;
using System.Text;

namespace API.Services.Exceptions {
    class InconsistentRequestException : Exception {
        public InconsistentRequestException() : base() {
        }

        public InconsistentRequestException(string message) : base(message) {
        }
    }
}
