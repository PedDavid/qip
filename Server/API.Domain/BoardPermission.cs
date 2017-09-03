using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public enum BoardPermission {
        // Required Permission Increase Order
        View = 1,
        Edit = 2,
        Owner = 3
    }
}
