using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using Authorization;
using Authorization.Extensions;
using Authorization.Resources;
using IODomain.Extensions;
using IODomain.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImageService imageService, IAuthorizationService authorizationService, ILogger<ImagesController> logger) {
            _imageService = imageService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.ListImagesNotAuthorized, "GetAll({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.ListImages, "Listing all Images");
            IEnumerable<Image> images = await _imageService.GetAllAsync(boardId);

            return Ok(images.Select(ImageExtensions.Out));
        }

        [HttpGet("{id}", Name = "GetImage")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy)) {
                _logger.LogWarning(LoggingEvents.GetImageNotAuthorized, "GetById({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            _logger.LogInformation(LoggingEvents.GetImage, "Getting Image {id}", id);
            Image image = await _imageService.GetAsync(id, boardId);

            if(image == null) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.GetImageNotFound, "GetById({id}, {boardId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            return Ok(image.Out());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(long boardId, [FromBody] InCreateImage inImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.InsertImageNotAuthorized, "Create({boardId}) NOT AUTHORIZED {user_id}", boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inImage == null) {
                _logger.LogDebug(LoggingEvents.InsertImageWithoutBody, "Create({boardId}) WITHOUT BODY", boardId);
                return BadRequest();
            }

            if(inImage.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                _logger.LogDebug(LoggingEvents.InsertImageWrongBoardId, "Create({boardId}) WRONG BOARD ID {wrongBoardId}", boardId, inImage.BoardId);
                return BadRequest();
            }

            Image image = new Image().In(inImage);

            await _imageService.CreateAsync(image);
            _logger.LogInformation(LoggingEvents.InsertImage, "Image {id} of Board {boardId} Created", image.Id, boardId);

            return CreatedAtRoute("GetImage", new { id = image.Id, boardId = boardId }, image.Out());
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InUpdateImage inImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.UpdateImageNotAuthorized, "Update({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(inImage == null) {
                _logger.LogDebug(LoggingEvents.UpdateImageWithoutBody, "Update({id}, {boardId}) WITHOUT BODY", id, boardId);
                return BadRequest();
            }

            if(inImage.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inImage.Id}"
                _logger.LogDebug(LoggingEvents.UpdateImageWrongBoardId, "Update({id}, {boardId}) WRONG ID {wrongId}", id, boardId, inImage.Id);
                return BadRequest();
            }

            if(inImage.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inImage.BoardId}"
                _logger.LogDebug(LoggingEvents.UpdateImageWrongBoardId, "Update({id}, {boardId}) WRONG BOARD ID {wrongBoardId}", id, boardId, inImage.BoardId);
                return BadRequest();
            }

            Image image = await _imageService.GetAsync(id, boardId);
            if(image == null) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.UpdateImageNotFound, "Update({id}, {boardId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            image.In(inImage);

            await _imageService.UpdateAsync(image);
            _logger.LogInformation(LoggingEvents.UpdateImage, "Image {id} of Board {boardId} Updated", id, boardId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy)) {
                _logger.LogWarning(LoggingEvents.DeleteImageNotAuthorized, "Delete({id}, {boardId}) NOT AUTHORIZED {user_id}", id, boardId, User.GetNameIdentifier());
                return Challenge();
            }

            if(!await _imageService.ExistsAsync(id, boardId)) {
                //$"The image with id {id}, belonging to board with id {boardId}, does not exist"
                _logger.LogWarning(LoggingEvents.DeleteImageNotFound, "Delete({id}, {boarId}) NOT FOUND", id, boardId);
                return NotFound();
            }

            await _imageService.DeleteAsync(id, boardId);
            _logger.LogInformation(LoggingEvents.DeleteImage, "Image {id} of Board {boardId} Deleted", id, boardId);

            return NoContent();
        }
    }
}
