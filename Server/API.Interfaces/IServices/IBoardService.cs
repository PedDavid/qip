﻿using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IBoardService : IGenericSearchableService<InBoard, OutBoard> {
    }
}