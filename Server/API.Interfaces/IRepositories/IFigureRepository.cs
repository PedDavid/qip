using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IFigureRepository<T> {
        Task<long> Add(T figure);
        Task<IEnumerable<T>> GetAll(long boardId);
        Task<T> Find(long id, long boardId);
        Task Remove(long id, long boardId);
        Task Update(T figure);
        Task PartialUpdate(T figure);
    }
}
