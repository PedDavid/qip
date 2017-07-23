using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Interfaces.IServices {
    public interface IUserService {
        Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10);

        Task<OutUser> GetById(long id);

        Task<OutUser> Create(InUser inputUser);

        Task<OutUser> Update(long id, InUser inputUser);

        Task Delete(long id);
    }
}
