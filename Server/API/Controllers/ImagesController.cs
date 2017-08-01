using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces.IRepositories;
using API.Domain;
using IODomain.Input;
using IODomain.Extensions;
using Microsoft.Extensions.Caching.Memory;
using API.Services;
using API.Services.Extensions;
using API.Interfaces.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class ImagesController : Controller {
        private readonly IBoardRepository _boardRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IFigureIdService _figureIdService;

        public ImagesController(IBoardRepository boardRepository, IImageRepository imageRepository, IFigureIdService figureIdService) {
            _boardRepository = boardRepository;
            _imageRepository = imageRepository;
            _figureIdService = figureIdService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _boardRepository.ExistsAsync(boardId)) {
                return NotFound();
            }

            IEnumerable<Image> images = await _imageRepository.GetAllAsync(boardId);

            return Json(images.Select(ImageExtensions.Out));
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

            IFigureIdGenerator idGen = await _figureIdService.GetOrCreateFigureIdGeneratorAsync(boardId);

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
