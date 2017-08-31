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
using System.Security.Claims;
using System.Linq;
using Authorization.Extensions;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ServicesExceptionFilter]
    public class BoardsController : Controller {
        private readonly IBoardService _boardService;
        private readonly IAuthorizationService _authorizationService;

        public BoardsController(IBoardService boardService, IAuthorizationService authorizationService) {
            _boardService = boardService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize("Administrator")]
        public Task<IEnumerable<OutBoard>> GetAllAsync(string search, long index = 0, long size = 10) {
            return _boardService.GetAllAsync(index, size, search);
        }

        [HttpGet("{id}", Name = "GetBoard")]
        [Authorize("Administrator")]
        public Task<OutBoard> GetByIdAsync(long id) {
            return _boardService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InBoard inputBoard) {
            OutBoard board = await _boardService.CreateAsync(inputBoard, User.GetNameIdentifier());
            return CreatedAtRoute("GetBoard", new { id = board.Id }, board);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InBoard inputBoard) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            await _boardService.UpdateAsync(id, inputBoard);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(id), Policies.BoardIsOwnPolicy))
                return new ChallengeResult();

            await _boardService.DeleteAsync(id);
            return new NoContentResult();
        }
    }
}
