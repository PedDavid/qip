using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services {
    class ImageService : IImageService {
        private readonly IImageRepository _imageRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IFigureIdService _figureIdService;

        public ImageService(IImageRepository imageRepository, IBoardRepository boardRepository, IFigureIdService figureIdService) {
            _imageRepository = imageRepository;
            _boardRepository = boardRepository;
            _figureIdService = figureIdService;
        }

        public async Task CreateAsync(Image image, bool autoGenerateId) {
            if(image == null) {
                throw new ArgumentNullException("Argument image can not be null");
            }

            if(!await _boardRepository.ExistsAsync(image.BoardId)) {
                throw new NotFoundException($"The Board with id {image.BoardId} not exists");
            }

            if(autoGenerateId) {
                IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(image.BoardId);

                image.Id = idGen.NewId();
            }

            await _imageRepository.AddAsync(image);
        }

        public Task DeleteAsync(long id, long boardId) {
            return _imageRepository.RemoveAsync(id, boardId);
        }

        public Task<bool> ExistsAsync(long id, long boardId) {
            return _imageRepository.ExistsAsync(id, boardId);
        }

        public async Task<IEnumerable<Image>> GetAllAsync(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            return await _imageRepository.GetAllAsync(boardId);
        }

        public Task<Image> GetAsync(long id, long boardId) {
            return _imageRepository.FindAsync(id, boardId);
        }

        public Task UpdateAsync(Image image) {
            if(image == null) {
                throw new ArgumentNullException("Argument image can not be null");
            }

            return _imageRepository.UpdateAsync(image);
        }
    }
}
