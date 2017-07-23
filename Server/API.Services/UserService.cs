using API.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;
using API.Domain;
using IODomain.Extensions;
using API.Interfaces.IRepositories;
using System.Linq;
using API.Services.Exceptions;

namespace API.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<OutUser> Create(InUser inputUser) {
            if(inputUser == null) {
                throw new MissingInputException();
            }

            ValidadeFields(inputUser);

            //TODO Check if username is unique

            User user = new User().In(inputUser);
            long id = await _userRepository.AddAsync(user);

            OutUser outUser = user.Out();
            outUser.Id = id;

            return outUser;
        }

        public async Task Delete(long id) {
            User user = await _userRepository.FindAsync(id);

            if(user == null) {
                throw new NotFoundException($"The User with id {id} not exists");
            }

            await _userRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<OutUser>> GetAll(string search, long index = 0, long size = 10) {
            IEnumerable<User> users = await _userRepository.GetAllAsync(search, index, size);

            return users.Select(UserExtensions.Out);
        }

        public async Task<OutUser> GetById(long id) {
            User user = await _userRepository.FindAsync(id);

            return user?.Out();
        }

        public async Task<OutUser> Update(long id, InUser inputUser) {
            if(inputUser.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputUser.Id}"
                );
            }

            if(inputUser == null) {
                throw new MissingInputException();
            }

            ValidadeFields(inputUser);

            User user = await _userRepository.FindAsync(id);
            if(user == null) {
                throw new NotFoundException($"The User with id {id} not exists");
            }

            user.In(inputUser);

            await _userRepository.UpdateAsync(user);

            return user.Out();
        }

        private void ValidadeFields(InUser inUser) {
            Dictionary<string, object> fieldsToValidate = new Dictionary<string, object>() {
                {"UserName", inUser.UserName},
                {"PwdHash", inUser.PwdHash},
                {"PwdSalt", inUser.PwdSalt},
                {"Name", inUser.Name}
            };

            string[] invalidFields = fieldsToValidate
                                        .Where(pair => pair.Value == null)
                                        .Select(pair => pair.Key)
                                        .ToArray();

            if(invalidFields.Length != 0) {
                string separator = ", ";
                int sepSize = separator.Length;

                string msg = invalidFields.Aggregate(
                    new StringBuilder("The following fields are missing: "),
                    (sb, s) => {
                        sb.Append(s);
                        sb.Append(separator);
                        return sb;
                    },
                    sb => {
                        sb.Remove(sb.Length - sepSize, sepSize);
                        return sb.ToString();
                    }
                );

                throw new InvalidInputException(msg);
            }
        }
    }
}
