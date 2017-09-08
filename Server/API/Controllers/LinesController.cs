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
    public class LinesController : Controller {
        private readonly ILineService _lineService;
        private readonly IAuthorizationService _authorizationService;

        public LinesController(ILineService lineService, IAuthorizationService authorizationService) {
            _lineService = lineService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return Challenge();

            IEnumerable<Line> lines = await _lineService.GetAllAsync(boardId);

            return Ok(lines.Select(LineExtensions.Out));
        }

        [HttpGet("{id}", Name = "GetLine")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.ReadBoardPolicy))
                return Challenge();

            Line line = await _lineService.GetAsync(id, boardId);

            if(line == null) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            return Ok(line.Out());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(long boardId, [FromBody] InCreateLine inLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(inLine.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                return BadRequest();
            }

            Line line = new Line().In(inLine);

            await _lineService.CreateAsync(line);

            return CreatedAtRoute("GetLine", new { id = line.Id, boardId = boardId }, line.Out());
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(long id, long boardId, [FromBody] InUpdateLine inLine) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(inLine.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLine.Id}"
                return BadRequest();
            }

            if(inLine.BoardId != boardId) {
                //$"The board id present on update is different of the expected. {Environment.NewLine}Expected: {boardId}{Environment.NewLine}Current: {inLine.BoardId}"
                return BadRequest();
            }

            Line line = await _lineService.GetAsync(id, boardId);
            if(line == null) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            line.In(inLine);

            await _lineService.UpdateAsync(line);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(long id, long boardId) {
            if(!await _authorizationService.AuthorizeAsync(User, new BoardRequest(boardId), Policies.WriteBoadPolicy))
                return Challenge();

            if(!await _lineService.ExistsAsync(id, boardId)) {
                //$"The line with id {id}, belonging to board with id {boardId}, does not exist"
                return NotFound();
            }

            await _lineService.DeleteAsync(id, boardId);

            return NoContent();
        }
    }
}
