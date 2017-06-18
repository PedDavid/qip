using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Models.Output;
using API.Models.Input;
using API.Models.IRepositories;
using API.Models.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class ImagesController : Controller {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository) {
            this._imageRepository = imageRepository;
        }

        [HttpGet]
        public IEnumerable<OutImage> GetAll(long boardId) {
            return _imageRepository
                .GetAll(boardId)
                .Select(ImageExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public IActionResult GetById(long id, long boardId) {
            Image image = _imageRepository.Find(id, boardId);
            if(image == null) {
                return NotFound();
            }
            return new ObjectResult(image.Out());
        }

        [HttpPost]
        public IActionResult Create(long boardId, [FromBody] InImage inputImage) {
            if(inputImage == null || inputImage.BoardId != boardId || !inputImage.Id.HasValue) {
                return BadRequest();
            }

            Image image = new Image(boardId, inputImage.Id.Value).In(inputImage);
            long id = _imageRepository.Add(image);

            inputImage.Id = id;
            return CreatedAtRoute("GetImage", new { id = id, boardId = boardId }, inputImage);
        }


        [HttpPut("{id}")]
        public IActionResult Update(long id, long boardId, [FromBody] InImage inputImage) {
            if(inputImage == null || inputImage.Id != id || inputImage.BoardId != boardId) {
                return BadRequest();
            }

            Image image = _imageRepository.Find(id, boardId);
            if(image == null) {
                return NotFound();
            }

            image.In(inputImage);

            _imageRepository.Update(image);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id, long boardId) {
            Image image = _imageRepository.Find(id, boardId);
            if(image == null) {
                return NotFound();
            }

            _imageRepository.Remove(id, boardId);
            return new NoContentResult();
        }
    }
}
