using API.Domain;
using API.Interfaces.IRepositories;
using API.Interfaces.IServices;
using API.Interfaces.ServicesExceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services {
    class ImageService : IImageService {
        private readonly IImageRepository _imageService;
        private readonly IBoardService _boardRepository;
        private readonly IFigureIdService _figureIdService;

        public ImageService(IImageRepository imageRepository, IBoardService boardService, IFigureIdService figureIdService) {
            _imageService = imageRepository;
            _boardRepository = boardService;
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

            await _imageService.AddAsync(image);
        }

        public Task DeleteAsync(long id, long boardId) {
            return _imageService.RemoveAsync(id, boardId);
        }

        public Task<bool> ExistsAsync(long id, long boardId) {
            return _imageService.ExistsAsync(id, boardId);
        }

        public async Task<IEnumerable<Image>> GetAllAsync(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            return await _imageService.GetAllAsync(boardId);
        }

        public Task<Image> GetAsync(long id, long boardId) {
            return _imageService.FindAsync(id, boardId);
        }

        public Task UpdateAsync(Image image) {
            if(image == null) {
                throw new ArgumentNullException("Argument image can not be null");
            }

            return _imageService.UpdateAsync(image);
        }
    }
}
