using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IFigureIdRepository {
        Task<long> GetMaxIdAsync();
    }
}
