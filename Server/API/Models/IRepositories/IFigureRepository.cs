using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.IRepositories {
    public interface IFigureRepository<T> {
        long Add(T figure);
        IEnumerable<T> GetAll(long boardId);
        T Find(long id, long boardId);
        void Remove(long id, long boardId);
        void Update(T figure);
        void PartialUpdate(T figure);
    }
}
