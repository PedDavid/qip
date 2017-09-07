using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IFigureService<F> {
        Task<IEnumerable<F>> GetAllAsync(long boardId);

        Task<F> GetAsync(long id, long boardId);

        Task CreateAsync(F inputFigure);

        Task UpdateAsync(F inputFigure);

        Task DeleteAsync(long id, long boardId);

        Task<bool> ExistsAsync(long id, long boardId);
    }
}
