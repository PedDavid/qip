using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces.IRepositories;
using API.Domain;
using IODomain.Input;
using IODomain.Extensions;
using IODomain.Output;
using Microsoft.Extensions.Caching.Memory;
using API.Services;
using API.Services.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class ImagesController : Controller {
        private readonly IImageRepository _imageRepository;
        private readonly IFigureIdRepository _figureIdRepository;
        private readonly IMemoryCache _memoryCache;

        public ImagesController(IImageRepository imageRepository, IFigureIdRepository figureIdRepository, IMemoryCache memoryCache) {
            this._imageRepository = imageRepository;
            _memoryCache = memoryCache;
            _figureIdRepository = figureIdRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<OutImage>> GetAll(long boardId) {
            IEnumerable<Image> images = await _imageRepository.GetAllAsync(boardId);

            return images.Select(ImageExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> GetById(long id, long boardId) {
            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                return NotFound();
            }
            return new ObjectResult(image.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InImage inputImage) {
            if(inputImage == null || inputImage.BoardId != boardId) {
                return BadRequest();
            }

            FigureIdGenerator idGen = await _memoryCache.GetFigureIdGenerator(_figureIdRepository, boardId);

            Image image = new Image(boardId, idGen.NewId()).In(inputImage);
            long id = await _imageRepository.AddAsync(image);

            inputImage.Id = id;
            return CreatedAtRoute("GetImage", new { id = id, boardId = boardId }, inputImage);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InImage inputImage) {
            if(inputImage == null || inputImage.Id != id || inputImage.BoardId != boardId) {
                return BadRequest();
            }

            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                return NotFound();
            }

            image.In(inputImage);

            await _imageRepository.UpdateAsync(image);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, long boardId) {
            Image image = await _imageRepository.FindAsync(id, boardId);
            if(image == null) {
                return NotFound();
            }

            await _imageRepository.RemoveAsync(id, boardId);
            return new NoContentResult();
        }
    }
}
