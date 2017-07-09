using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSockets.Models {
    public enum Action {
        PING,
        CREATE_IMAGE,
        DELETE_IMAGE,
        ALTER_IMAGE,
        CREATE_LINE,
        DELETE_LINE,
        ALTER_LINE
    }
}
