﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domain {
    public enum BoardPermission {
        // Required Permission Increase Order
        View = 0,
        Edit = 1,
        Owner = 2
    }
}
