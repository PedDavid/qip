using System;
using System.Collections.Generic;
using System.Text;

namespace API.Interfaces.IRepositories {
    public interface IFigureIdRepository {
        long GetMaxId();
    }
}
