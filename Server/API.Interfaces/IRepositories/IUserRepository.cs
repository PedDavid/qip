using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.IRepositories {
    public interface IUserRepository : IDefaultSearchableRepository<User> {
    }
}
