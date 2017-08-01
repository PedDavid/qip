using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class ImagesController : Controller {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService) {
            _imageService = imageService;
        }

        [HttpGet]
        public Task<IEnumerable<OutImage>> GetAll(long boardId) {
            return _imageService.GetAllAsync(boardId);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public Task<OutImage> GetById(long id, long boardId) {
            return _imageService.GetAsync(id, boardId);
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InImage inputImage) {
            OutImage image = await _imageService.CreateAsync(boardId, inputImage);
            return CreatedAtRoute("GetImage", new { id = image.Id, boardId = boardId }, image);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InImage inputImage) {
            await _imageService.UpdateAsync(id, boardId, inputImage);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, long boardId) {
            await _imageService.DeleteAsync(id, boardId);
            return new NoContentResult();
        }
    }
}
