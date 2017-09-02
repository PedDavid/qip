using Authorization;
using Authorization.Resources;
using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
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
                return new ChallengeResult();

            IEnumerable < OutImage > images = await _imageService.GetAllAsync(boardId);

            return Ok(images);
        }

        [HttpGet("{id}", Name = "GetImage")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return new ChallengeResult();

            OutImage image = await _imageService.GetAsync(id, boardId);

            return Ok(image);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(long boardId, [FromBody] InImage inputImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            OutImage image = await _imageService.CreateAsync(boardId, inputImage);

            return CreatedAtRoute("GetImage", new { id = image.Id, boardId = boardId }, image);
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InImage inputImage) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            await _imageService.UpdateAsync(id, boardId, inputImage);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            await _imageService.DeleteAsync(id, boardId);

            return new NoContentResult();
        }
    }
}
