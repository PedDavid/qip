using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IDefaultRepository<T> {
        long Add(T t);
        IEnumerable<T> GetAll();
        T Find(long id);
        void Remove(long id);
        void Update(T t);
        void PartialUpdate(T t);
    }
}
