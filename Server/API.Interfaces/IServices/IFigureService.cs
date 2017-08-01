using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IFigureService<IF,OF> {
        Task<IEnumerable<OF>> GetAllAsync(long boardId);

        Task<OF> GetAsync(long id, long boardId);

        Task<OF> CreateAsync(long boardId, IF inputFigure);

        Task<OF> UpdateAsync(long id, long boardId, IF inputFigure);

        Task DeleteAsync(long id, long boardId);
    }
}
