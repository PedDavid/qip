using API.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;
using API.Services.Exceptions;
using API.Interfaces.IRepositories;
using API.Domain;
using IODomain.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using API.Services.Utils;

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

        public async Task<OutImage> CreateAsync(long boardId, InImage inImage) {
            if(inImage == null) {
                throw new MissingInputException();
            }

            Validator<InImage>.Valid(inImage, GetCreateValidationConfigurations());

            if(inImage.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                );
            }

            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);

            Image image = new Image(boardId, idGen.NewId()).In(inImage);
            long id = await _imageRepository.AddAsync(image);

            OutImage outImage = image.Out();
            outImage.Id = id;

            return outImage;
        }

        private static ValidatorConfiguration<InImage> GetCreateValidationConfigurations() {
            return new ValidatorConfiguration<InImage>()
                .NotNull("BoardId", i => i.BoardId)
                .NotNull("Src", i => i.Src)
                .NotNull("Height", i => i.Height)
                .NotNull("Width", i => i.Width)
                .NotNull("Origin", i => i.Origin)
                .NotNull("Origin.X", i => i.Origin.X)
                .NotNull("Origin.Y", i => i.Origin.Y);
        }

        public async Task DeleteAsync(long id, long boardId) {
            if(!await _imageRepository.ExistsAsync(id, boardId)) {
                throw new NotFoundException($"The image with id {id}, belonging to board with id {boardId}, does not exist");
            }

            await _imageRepository.RemoveAsync(id, boardId);
        }

        public async Task<IEnumerable<OutImage>> GetAllAsync(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                throw new NotFoundException($"The Board with id {boardId} not exists");
            }

            IEnumerable<Image> images = await _imageRepository.GetAllAsync(boardId);

            return images.Select(ImageExtensions.Out);
        }

        public async Task<OutImage> GetAsync(long id, long boardId) {
            Image image = await _imageRepository.FindAsync(id, boardId);

            if(image == null) {
                throw new NotFoundException($"The image with id {id}, belonging to board with id {boardId}, does not exist");
            }

            return image.Out();
        }

        public async Task<OutImage> UpdateAsync(long id, long boardId, InImage inImage) {
            if(inImage == null) {
                throw new MissingInputException();
            }

            Validator<InImage>.Valid(inImage, GetUpdateValidationConfigurations());

            if(inImage.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inImage.Id}"
                );
            }

            if(inImage.BoardId != boardId) {
                throw new InconsistentRequestException(
                    $"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                );
            }

            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                throw new NotFoundException($"The image with id {id}, belonging to board with id {boardId}, does not exist");
            }

            image.In(inImage);

            await _imageRepository.UpdateAsync(image);

            return image.Out();
        }

        private static ValidatorConfiguration<InImage> GetUpdateValidationConfigurations() {
            return GetCreateValidationConfigurations()
                .NotNull("Id", i => i.Id);
        }
    }
}
