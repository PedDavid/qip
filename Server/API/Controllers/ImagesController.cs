using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Route("api/boards/{boardId}/figures/[controller]")]
    public class ImagesController : Controller {
        private readonly IImageService _imageService;
        private readonly IAuthorizationService _authorizationService;

        public ImagesController(IImageService imageService, IAuthorizationService authorizationService) {
            _imageService = imageService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return Challenge();

            IEnumerable<Image> images = await _imageService.GetAllAsync(boardId);

            return Ok(images.Select(ImageExtensions.Out));
        }

        [HttpGet("{id}", Name = "GetImage")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return Challenge();

            Image image = await _imageService.GetAsync(id, boardId);

            if(image == null) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            return Ok(image.Out());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(long boardId, [FromBody] InCreateImage inImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(inImage.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                return BadRequest();
            }

            Image image = new Image().In(inImage);

            await _imageService.CreateAsync(image);

            return CreatedAtRoute("GetImage", new { id = image.Id, boardId = boardId }, image.Out());
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InUpdateImage inImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(inImage.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inImage.Id}"
                return BadRequest();
            }

            if(inImage.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                return BadRequest();
            }

            Image image = await _imageService.GetAsync(id, boardId);
            if(image == null) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            image.In(inImage);

            await _imageService.UpdateAsync(image);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(!await _imageService.ExistsAsync(id, boardId)) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            await _imageService.DeleteAsync(id, boardId);

            return NoContent();
        }
    }
}
