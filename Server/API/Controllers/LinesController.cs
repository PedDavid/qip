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
    public class LinesController : Controller {
        private readonly ILineService _lineService;
        private readonly IAuthorizationService _authorizationService;

        public LinesController(ILineService lineService, IAuthorizationService authorizationService) {
            _lineService = lineService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return new ChallengeResult();

            IEnumerable<OutLine> lines = await _lineService.GetAllAsync(boardId);

            return Ok(lines);
        }

        [HttpGet("{id}", Name = "GetLine")]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return new ChallengeResult();

            OutLine line = await _lineService.GetAsync(id, boardId);

            return Ok(line);
        }

        [HttpPost]
        public async Task<IActionResult> Create(long boardId, [FromBody] InLine inputLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            OutLine line = await _lineService.CreateAsync(boardId, inputLine);

            return CreatedAtRoute("GetLine", new { id = line.Id, boardId = boardId }, line);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InLine inputLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            await _lineService.UpdateAsync(id, boardId, inputLine);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return new ChallengeResult();

            await _lineService.DeleteAsync(id, boardId);

            return new NoContentResult();
        }
    }
}
