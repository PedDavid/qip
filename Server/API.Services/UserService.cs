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
using API.Services.Utils;

namespace API.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<OutUser> CreateAsync(InUser inputUser) {
            if(inputUser == null) {
                throw new MissingInputException();
            }

            Validator<InUser>.Valid(inputUser, GetCreateValidationConfigurations());

            if(await _userRepository.UsernameExistsAsync(inputUser.UserName)) {
                throw new InvalidFieldsException($"The Username {inputUser.UserName} already exists");
            }

            User user = new User().In(inputUser);
            long id = await _userRepository.AddAsync(user);

            OutUser outUser = user.Out();
            outUser.Id = id;

            return outUser;
        }

        private static ValidatorConfiguration<InUser> GetCreateValidationConfigurations() {
            return new ValidatorConfiguration<InUser>()
                .NotNull("Name", i => i.Name)
                .NotNull("UserName", i => i.UserName)
                .NotNull("PwdHash", i => i.PwdHash)
                .NotNull("PwdSalt", i => i.PwdSalt);
        }

        public async Task DeleteAsync(long id) {
            if(!await _userRepository.ExistsAsync(id)) {
                throw new NotFoundException($"The User with id {id} not exists");
            }

            await _userRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<OutUser>> GetAllAsync(string search, long index = 0, long size = 10) {
            if(search == null)
                return await GetAllAsync(index, size);

            IEnumerable<User> users = await _userRepository.GetAllAsync(search, index, size);

            return users.Select(UserExtensions.Out);
        }

        public async Task<IEnumerable<OutUser>> GetAllAsync(long index = 0, long size = 10) {
            IEnumerable<User> users = await _userRepository.GetAllAsync(index, size);

            return users.Select(UserExtensions.Out);
        }

        public async Task<OutUser> GetAsync(long id) {
            User user = await _userRepository.FindAsync(id);

            if(user == null) {
                throw new NotFoundException($"The User with id {id} not exists");
            }

            return user.Out();
        }

        public async Task<OutUser> UpdateAsync(long id, InUser inputUser) {
            if(inputUser == null) {
                throw new MissingInputException();
            }

            Validator<InUser>.Valid(inputUser, GetUpdateValidationConfigurations());

            if(inputUser.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputUser.Id}"
                );
            }

            User user = await _userRepository.FindAsync(id);
            if(user == null) {
                throw new NotFoundException($"The User with id {id} not exists");
            }

            user.In(inputUser);

            await _userRepository.UpdateAsync(user);

            return user.Out();
        }

        private static ValidatorConfiguration<InUser> GetUpdateValidationConfigurations() {
            return GetCreateValidationConfigurations()
                .NotNull("Id", i => i.Id);
        }
    }
}
