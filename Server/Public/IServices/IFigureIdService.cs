﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IFigureIdService {
        Task<IFigureIdGenerator> GetOrCreateFigureIdGeneratorAsync(long boardId);
    }
}
