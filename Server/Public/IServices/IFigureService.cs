﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Public.IServices {
    public interface IFigureService<F> {
        Task<IEnumerable<F>> GetAllAsync(long boardId);

        Task<F> GetAsync(long id, long boardId);

        Task CreateAsync(F inputFigure, bool autoGenerateId = true);

        Task UpdateAsync(F inputFigure);

        Task DeleteAsync(long id, long boardId);

        Task<bool> ExistsAsync(long id, long boardId);
    }
}
