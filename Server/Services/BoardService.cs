using QIP.Domain;
using QIP.Public.IRepositories;
using QIP.Public.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIP.Services {
    public class BoardService : IBoardService {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository) {
            _boardRepository = boardRepository;
        }

        public Task CreateAsync(Board board, string userId) {
            if(board == null) {
                throw new ArgumentNullException("Argument board can not be null");
            }

            if(userId == null) {//Caso seja criado por um user anónimo
                board.BasePermission = BoardPermission.Edit;//Forçar o quadro a ter permissões de escrita publicas
            }

            return _boardRepository.AddAsync(board, userId);
        }

        public Task DeleteAsync(long id) {
            return _boardRepository.RemoveAsync(id);
        }

        public Task<bool> ExistsAsync(long id) {
            return _boardRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Board>> GetAllAsync(long index, long size, string search) {
            return search != null ? 
                await _boardRepository.GetAllAsync(index, size) : 
                await _boardRepository.GetAllAsync(index, size, search);
        }

        public Task<IEnumerable<Board>> GetAllAsync(long index, long size) {
            return GetAllAsync(index, size, null);
        }

        public Task<Board> GetAsync(long id) {
            return _boardRepository.FindAsync(id);
        }

        public Task UpdateAsync(Board board) {
            if(board == null) {
                throw new ArgumentNullException("Argument board can not be null");
            }

            return _boardRepository.UpdateAsync(board);
        }
    }
}
