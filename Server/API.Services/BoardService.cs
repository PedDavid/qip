﻿using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using API.Services.Utils;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    class BoardService : IBoardService {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository) {
            _boardRepository = boardRepository;
        }

        public async Task<OutBoard> CreateAsync(InBoard inputBoard) {
            if(inputBoard == null) {
                throw new MissingInputException();
            }

            Validator<InBoard>.Valid(inputBoard, GetCreateValidationConfigurations());

            Board board = new Board().In(inputBoard);
            long id = await _boardRepository.AddAsync(board);

            return board.Out(id);
        }

        private static ValidatorConfiguration<InBoard> GetCreateValidationConfigurations() {
            return new ValidatorConfiguration<InBoard>()
                .NotNull("Name", i => i.Name)
                .NotNull("MaxDistPoints", i => i.MaxDistPoints);
        }

        public async Task DeleteAsync(long id) {
            if(!await _boardRepository.ExistsAsync(id)) {
                throw new NotFoundException($"The Board with id {id} not exists");
            }

            await _boardRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<OutBoard>> GetAllAsync(long index, long size, string search) {
            if(search == null)
                return await GetAllAsync(index, size);

            IEnumerable<Board> boards = await _boardRepository.GetAllAsync(index, size, search);

            return boards.Select((Func<Board, OutBoard>)BoardExtensions.Out);
        }

        public async Task<IEnumerable<OutBoard>> GetAllAsync(long index, long size) {
            IEnumerable<Board> boards = await _boardRepository.GetAllAsync(index, size);

            return boards.Select((Func<Board, OutBoard>)BoardExtensions.Out);
        }

        public async Task<OutBoard> GetAsync(long id) {
            Board board = await _boardRepository.FindAsync(id);

            if(board == null) {
                throw new NotFoundException($"The Board with id {id} not exists");
            }

            return board.Out();
        }

        public async Task<OutBoard> UpdateAsync(long id, InBoard inputBoard) {
            if(inputBoard == null) {
                throw new MissingInputException();
            }

            Validator<InBoard>.Valid(inputBoard, GetUpdateValidationConfigurations());

            if(inputBoard.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inputBoard.Id}"
                );
            }

            Board board = await _boardRepository.FindAsync(id);
            if(board == null) {
                throw new NotFoundException($"The Board with id {id} not exists");
            }

            board.In(inputBoard);

            await _boardRepository.UpdateAsync(board);

            return board.Out();
        }

        private static ValidatorConfiguration<InBoard> GetUpdateValidationConfigurations() {
            return GetCreateValidationConfigurations()
                .NotNull("Id", i => i.Id);
        }
    }
}
