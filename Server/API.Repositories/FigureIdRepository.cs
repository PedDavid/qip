using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Repositories
{
    public class FigureIdRepository : IFigureIdRepository {
        public long GetMaxId() {
            return 0;//TODO
        }
    }
}
